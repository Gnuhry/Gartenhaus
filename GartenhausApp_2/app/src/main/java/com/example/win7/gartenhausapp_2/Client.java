package com.example.win7.gartenhausapp_2;

import android.os.AsyncTask;
import android.util.Log;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.io.PrintStream;
import java.io.PrintWriter;
import java.net.Socket;

public class Client {
    int Port=5000;
    String IP;
    /*private volatile static String message;
    private Thread receiveThread;

    /**
     * Construcor
     *
     * @param IP IP-Adress of Server
     */
    Client(String IP) {
        this.IP=IP;
        //receiveThread = new Thread(new Receive());
        //receiveThread.start();
    }

    /**
     * Constructor 2
     *
     * @param IP   IP-Adress of Server
     * @param Port Port of Server
     */
    Client(String IP, int Port) {
        this.IP=IP;
        this.Port = Port;
       // receiveThread = new Thread(new Receive());
        //receiveThread.start();
    }
private class ClientThread extends Thread{
        String message,erg;
        public ClientThread(String message){
            this.message=message+"<EOF>";
            erg="";
        }
        @Override
        public void run(){
            try {
                Socket socket;
                //do{
                    socket=new Socket(IP,Port);
                //}while(!socket.isConnected());
                if(!socket.isConnected()){
                    throw new IOException();
                }
                InputStream inputStream = socket.getInputStream();
                OutputStream outputStream = socket.getOutputStream();
                PrintWriter printWriter = new PrintWriter(new OutputStreamWriter(outputStream));
                printWriter.write(message);
                printWriter.flush();
                //PrintStream printStream = new PrintStream(outputStream);
                //printStream.print(message);
                Log.e("Client2",message);
                BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(inputStream));
                StringBuilder sb = new StringBuilder();
                do {
                    sb.append(bufferedReader.readLine());
                } while (sb.indexOf("<EOF>") < 0);
                erg=sb.toString().substring(0,sb.toString().length()-5);
                Log.e("Client2",erg);
                socket.close();
                if(erg==""){
                    throw new IOException();
                }
            } catch (IOException e) {
                e.printStackTrace();
                erg="Error";
            }
        }

    public String getErg() {
        return erg;
    }
}





    /**
     * Receive Thread
     * Handle incoming message
     */
    /*  private class Receive implements Runnable {
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
*/
    /**
     * Stop Receive Thread
     */
   /* public void Stop() {
        receiveThread.interrupt();
    }*/

    /**
     * Send to server
     *
     * @param command command for Server
     * @return Answer from Server
     */
    public String Send(String command) {
        String erg="";
        ClientThread ct=new ClientThread(command);
        new Thread(ct).start();
        try {
            Thread.sleep(1500);
        } catch (InterruptedException e) {
        }
        int deadcounter=0;
        do{
            erg=ct.getErg();
        }while(erg==""&&++deadcounter<500);
        if(erg==""){
            erg="Error";
        }
        return erg;
       /* try {
            if (!socket.isConnected()) {
                return "Error";
            }
        } catch (Exception ex) {
            return "Error";
        }
        //sending
        //new ClientTask(command + "_<EOF>").execute();
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
        return ret;*/
    }

    /**
     * asynchron Class for Sending to Server
     */
   // private static class ClientTask extends AsyncTask<Void, Void, Void> {

        /**
         * Constructor
         */
  /*      String command;

        private ClientTask(String command) {
            this.command = command;
        }

        /**
         * Sending in background
         */
    /*    @Override
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
    }*/
}
