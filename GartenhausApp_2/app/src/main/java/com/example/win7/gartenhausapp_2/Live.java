package com.example.win7.gartenhausapp_2;

import android.os.AsyncTask;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Switch;
import android.widget.TextView;
import android.widget.Toast;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;

public class Live extends AppCompatActivity {

    String ip;
    ClientTask clientTask;
    Boolean startstop;
    int ID;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_live);
        ID = getIntent().getIntExtra("ID", -1); //read ID
        ip = MainActivity.client.Send("get arduino all_" + ID).split("_")[1];
        try {
            Thread.sleep(500);
        } catch (InterruptedException e) {
            Toast.makeText(this, "Nope", Toast.LENGTH_SHORT).show();
        }
        Log.e("live2",ip);
        startstop=true;
    }

    public void btnStartStop(View view) {
        if(startstop){
            startstop=false;
            findViewById(R.id.swheater).setEnabled(true);
            findViewById(R.id.swsprayer).setEnabled(true);
            findViewById(R.id.swpump).setEnabled(true);
            findViewById(R.id.swlight).setEnabled(true);
            findViewById(R.id.swcooler).setEnabled(true);
            findViewById(R.id.swshutters).setEnabled(true);
            MainActivity.client.Send("live_" + ID);
            try {
                Thread.sleep(500);
            } catch (InterruptedException e) {
                Toast.makeText(this, "Nope", Toast.LENGTH_SHORT).show();
            }
//             boolean aa=true;
//            do{
//            try {
//                Log.e("live2","try_"+ip);
//                Socket s=new Socket(ip,5000);
//                PrintWriter writer = new PrintWriter(s.getOutputStream());
//                writer.write("_");
//                writer.flush();
//                writer.close();
//                s.close();
//                aa=false;
//            } catch (IOException e) {
//                e.printStackTrace();
//            }
//            }while(aa);
            clientTask=new ClientTask(new TextView[]{findViewById(R.id.txVtemp),findViewById(R.id.txVhumid),findViewById(R.id.txVgroundHumid),findViewById(R.id.txVlight)},
                new Switch[]{findViewById(R.id.swheater),findViewById(R.id.swsprayer),findViewById(R.id.swpump),findViewById(R.id.swlight),findViewById(R.id.swcooler),findViewById(R.id.swshutters)});
        clientTask.execute();
        }
        else{
            startstop=true;
            findViewById(R.id.swheater).setEnabled(false);
            findViewById(R.id.swsprayer).setEnabled(false);
            findViewById(R.id.swpump).setEnabled(false);
            findViewById(R.id.swlight).setEnabled(false);
            findViewById(R.id.swcooler).setEnabled(false);
            findViewById(R.id.swshutters).setEnabled(false);
            clientTask.StopServer();
            /*boolean b = false;
            while(!b){
                Log.e("live2","try");
                try {
                    Socket socket = null;
                    socket = new Socket(ip, 5000);
                    String message="";
                    BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(socket.getInputStream()));
                    while (!b) {
                        message = bufferedReader.readLine();
                        if (message != null) b = true;
                    }
                }
                catch (Exception e) {
                    e.printStackTrace();
                }
            }*/
        }
    }

    private static class ClientTask extends AsyncTask<Void, Void, Void> {

        /**
         * Constructor
         */
        Boolean aa, first;
        PrintWriter writer;
        BufferedReader bufferedReader;
        TextView[] txV;
        Switch[] sw;

        private ClientTask(TextView[]txV_,Switch[]sw_) {
            aa = first = true;
            txV=txV_;
            sw=sw_;
        }

        public void StopServer() {
            aa = false;
        }

        /**
         * Sending in background
         */
        @Override
        protected Void doInBackground(Void... voids) {
            try {
                ServerSocket serverSocket = new ServerSocket(5000);
                while (aa) {
                    Socket socket = serverSocket.accept();
                    Log.e("live2",socket.getRemoteSocketAddress().toString());
                    bufferedReader = new BufferedReader(new InputStreamReader(socket.getInputStream()));
                    writer = new PrintWriter(socket.getOutputStream());
                    Boolean b = false;
                    String message = "";
                    while (aa) {
                        try {
                            b = false;
                            Log.e("live2","wait");
                            while (!b) {
                                message = bufferedReader.readLine();
                                if (message != null) b = true;
                            }
                            Log.e("live2",message);
                            String[] split = message.split("_");
                            txV[0].setText(split[0] + "°C");
                            txV[1].setText(split[1]);
                            txV[2].setText(split[2]);
                            txV[3].setText(split[3]);
                            if (first) {
                                char[] characters = split[4].toCharArray();
                                for (int f = 0; f < 6; f++) {
                                    sw[f].setChecked(characters[f] == '1');
                                }
                                first = false;
                            }
                            StringBuilder erg= new StringBuilder();
                            for(int f=0;f<6;f++){
                                if(sw[f].isChecked()){
                                    erg.append("1");
                                }
                                else{
                                    erg.append("0");
                                }
                            }
                            writer.println(erg.toString());
                        } catch (Exception ignored) {
                        }
                    }
                    while (!b) {
                        writer.println("live off");
                        message = bufferedReader.readLine();
                        if (message != null) b = true;
                    }
                    bufferedReader.close();
                    writer.close();
                    socket.close();
                    serverSocket.close();
                }
            } catch (Exception ignored) {

            }
            return null;
        }
    }
}
