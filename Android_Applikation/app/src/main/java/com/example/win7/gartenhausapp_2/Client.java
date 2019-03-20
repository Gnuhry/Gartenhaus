package com.example.win7.gartenhausapp_2;

import android.util.Log;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.Socket;

public class Client {
    private int Port;
    private String IP;

    Client(String IP, int Port) {
        this.IP = IP;
        this.Port = Port;
    }

//Thread zum Senden und Empfangen von Nachrichten
    private class ClientThread extends Thread {
        String message, erg;

        ClientThread(String message) {
            this.message = message + "<EOF>";
            erg = "";
        }

        @Override
        public void run() {
            try {
                Socket socket;
                socket = new Socket(IP, Port);
                if (!socket.isConnected()) {
                    throw new IOException();
                }
                InputStream inputStream = socket.getInputStream();
                OutputStream outputStream = socket.getOutputStream();
                PrintWriter printWriter = new PrintWriter(new OutputStreamWriter(outputStream));
                printWriter.write(message);//Nachricht senden
                printWriter.flush();//direktes lesen
                Log.e("Client2", "Get: " + message);
                BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(inputStream));
                StringBuilder sb = new StringBuilder();
                do {
                    sb.append(bufferedReader.readLine());
                } while (sb.indexOf("<EOF>") < 0);
                erg = sb.toString().substring(0, sb.toString().length() - 5);
                Log.e("Client2", "Set: " + erg);
                bufferedReader.close();
                socket.close();
                if (erg.equals("")) {
                    throw new IOException();
                }
            } catch (IOException e) {
                e.printStackTrace();
                erg = "Error";
            }
        }

        String getErg() {
            return erg;
        }
    }

//Nachricht Senden und Empfangen
//Falls Empfangen länger dauert als 500 ms wird das Warten abgebrochen
    public String Send(String command) {
        String erg;
        ClientThread ct = new ClientThread(command);
        new Thread(ct).start();
        try {
            Thread.sleep(1500);
        } catch (InterruptedException ignored) {
        }
        int deadcounter = 0;
        do {
            erg = ct.getErg();
        } while (erg.equals("") && ++deadcounter < 500);
        if (erg.equals("")) {
            erg = "Error";
        }
        return erg;
    }
}
