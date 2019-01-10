package com.example.win7.gartenhausapp_2;

import android.os.AsyncTask;
import android.util.Log;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;

public class Client {
    private static Socket socket;
    private static String IP;
    private int Port = 5000;
    private volatile static String message;
    private Thread receiveThread;

    /**
     * Construcor
     *
     * @param IP IP-Adress of Server
     */
    Client(String IP) {
        Client.IP = IP;
        receiveThread = new Thread(new Receive());
        receiveThread.start();
    }

    /**
     * Constructor 2
     *
     * @param IP   IP-Adress of Server
     * @param Port Port of Server
     */
    Client(String IP, int Port) {
        this.Port = Port;
        Client.IP = IP;
        receiveThread = new Thread(new Receive());
        receiveThread.start();
    }

    /**
     * Receive Thread
     * Handle incoming message
     */
    private class Receive implements Runnable {
        Boolean b = false;

        @Override
        public void run() {

            while (true) {
                try {
                    b = false;
                    socket = new Socket(IP, Port);
                    BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(socket.getInputStream()));
                    while (!b) {
                        message = bufferedReader.readLine();
                        if (message != null) b = true;
                    }
                } catch (IOException e) {
                    e.printStackTrace();

                }

            }
        }
    }

    /**
     * Stop Receive Thread
     */
    public void Stop() {
        receiveThread.interrupt();
    }

    /**
     * Send to server
     *
     * @param command command for Server
     * @return Answer from Server
     */
    public String Send(String command) {
        try {
            if (!socket.isConnected()) {
                return "Error";
            }
        } catch (Exception ex) {
            return "Error";
        }
        //sending
        new ClientTask(command + "_<EOF>").execute();
        //wait for answer
        while (message == null) {
        }
        try {
            socket.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
        String ret = message;
        message = null;
        return ret;
    }

    /**
     * asynchron Class for Sending to Server
     */
    private static class ClientTask extends AsyncTask<Void, Void, Void> {

        /**
         * Constructor
         */
        String command;

        private ClientTask(String command) {
            this.command = command;
        }

        /**
         * Sending in background
         */
        @Override
        protected Void doInBackground(Void... voids) {
            try {
                PrintWriter printWriter = new PrintWriter(socket.getOutputStream());
                printWriter.write(command);
                printWriter.flush();
            } catch (IOException e) {
                Log.e("Error3",
                        e.toString());
            }
            return null;
        }
    }
}
