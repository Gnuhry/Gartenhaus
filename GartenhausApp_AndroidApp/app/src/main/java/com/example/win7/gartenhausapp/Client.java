package com.example.win7.gartenhausapp;

import android.os.AsyncTask;
import android.util.Log;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;

//Objekt für TCP/IP Socket Kommunikation
public class Client {

    private static Socket socket; //Socket initalisieren
    private static String IP;      //IP initalisieren
    private static int Port=5000; //Serverport initalisieren
    private volatile static String message;
    private Thread thread;


    Client(String IP){ //Konstrukter
        Client.IP =IP;
        thread=new Thread(new Receive());//Receive Thread initalisieren
        thread.start();
    }
    Client(String IP,int Port){ //Konstrukter
        this.Port=Port;
        Client.IP =IP;
        thread=new Thread(new Receive());//Receive Thread initalisieren
        thread.start();
    }
    private class Receive implements Runnable{
        Boolean b=false;
        @Override
        public void run() {

            while(true){//endlos nach Kommunikation scuhen und auslesen
                try {
                    b=false;
                    socket=new Socket(IP,Port);
                    BufferedReader bufferedReader=new BufferedReader(new InputStreamReader(socket.getInputStream()));
                    while(!b){
                        message=bufferedReader.readLine();
//                        Log.e("Receive",message);
                        if(message!=null) b=true;
                    }
//                    bufferedReader.close();
                } catch (IOException e) {
                    e.printStackTrace();

                }

            }
        }
    }
//    private boolean Connection(){
//        Socket test=new Socket();
//        SocketAddress testA=new InetSocketAddress("8.8.8.8",53);
//        try {
//            test.connect(testA);
//            test.close();
//            return true;
//        } catch (IOException e) {
//            e.printStackTrace();
//        }
//        return false;
//    }
    public void Stop(){
        thread.interrupt();
    } //Client schließen
    public String Send(String command){ //An Server senden
        try{
        if(!socket.isConnected()) return "Error";} //Abfangen von Fehlern
        catch(Exception ex){return "Error";}
        new ClientTask(command+"_<EOF>").execute(); //Befehl senden

        while(message==null) //Auf Antwort warten
        {

        }
        try {
            socket.close();//Socket Kommunikation schließen
        } catch (IOException e) {
            e.printStackTrace();
        }
        String ret=message;
        message=null; //Message leeren
        return ret; //Antwort returnen
    }
    private static class ClientTask extends AsyncTask<Void,Void,Void> { //Asynchrone Klasse zum Senden

        String command;
        private ClientTask(String command) {
            this.command=command;
        }//Konstruker

        @Override
        protected Void doInBackground(Void... voids) {
            try {
                PrintWriter printWriter=new PrintWriter(socket.getOutputStream());
                printWriter.write(command); //sendern an Server
                printWriter.flush();
            } catch (IOException e) {
                Log.e("Error3",
                        e.toString());
            }
            return null;
        }


    }

}
