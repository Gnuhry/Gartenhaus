package com.example.win7.gartenhausapp_2;

import android.util.Log;
import android.widget.Switch;
import android.widget.TextView;
import android.widget.Toast;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;

public class Server {
    private ServerSocket serverSocket;
    private Switch[] sw;
    private boolean first, end;
    Live live;

    Server(Switch[] sw, Live live) {
        this.live = live;
        this.sw = sw;
        first = true;
    }

    public void Stop() {
        end = true;
    }

    public void Restart() {
        new Thread(new ServerThread()).start();
    }

    private class ServerThread extends Thread {

        @Override
        public void run() {
            end = false;
            try {
                serverSocket = new ServerSocket(5000);
                do {
                    Socket client = serverSocket.accept(); //Verbindung starten
                    new SocketAnswer(client, end).start();
                } while (!end);
            } catch (IOException ignored) {
            }
        }
    }

    private class SocketAnswer extends Thread {
        private Socket socket;
        private boolean end;

        SocketAnswer(Socket socket, boolean end) {
            this.socket = socket;
            this.end = end;
        }

        @Override
        public void run() {
            OutputStream outputStream;
            InputStream inputStream;
            try {
                inputStream = socket.getInputStream();
                outputStream = socket.getOutputStream();
                BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(inputStream));
                PrintWriter printWriter = new PrintWriter(new OutputStreamWriter(outputStream));
                StringBuilder sb = new StringBuilder();
                while (!end) {
                    do {
                        sb.append(bufferedReader.readLine());
                    } while (sb.indexOf("|") < 0);
                    Run x = new Run(sb.toString().split("_")); //Live Funktion Button Status holen
                    Log.e("Server2", "Get: " + sb.toString());
                    live.runOnUiThread(x);
                    try {
                        Thread.sleep(500);
                    } catch (InterruptedException e) {
                        Toast.makeText(live, "Nope", Toast.LENGTH_SHORT).show();
                    }
                    printWriter.print(x.GetString() + "|");
                    printWriter.flush();
                    Log.e("Server2", "Send: " + x.GetString() + "|");
                    sb.delete(0, sb.length() + 1);
                }
            } catch (IOException ex) {
                ex.printStackTrace();
            }
        }
    }

    public class Run implements Runnable {
        StringBuilder erg = new StringBuilder();
        String split[];

        Run(String split[]) {
            this.split = split;
        }

        @Override
        public void run() {
          //Einlesen von Live Funktion Button Status
            split[0] += "°C";
            ((TextView) live.findViewById(R.id.txVtemp)).setText(split[0]);
            ((TextView) live.findViewById(R.id.txVhumid)).setText(split[1]);
            ((TextView) live.findViewById(R.id.txVgroundHumid)).setText(split[2]);
            ((TextView) live.findViewById(R.id.txVlight)).setText(split[3]);
            Log.e("Server2", split[4]);
            if (first) {
                char[] characters = split[4].toCharArray();
                for (int f = 0; f < 6; f++) {
                    sw[f].setChecked(characters[f] == '1');
                }
                first = false;
            }
            if (end) {
                erg.append("live off");
                try {
                    serverSocket.close();
                } catch (IOException e) {
                    e.printStackTrace();
                }
            } else {
                for (int f = 0; f < 6; f++) {
                    if (sw[f].isChecked()) {
                        erg.append("1");
                    } else {
                        erg.append("0");
                    }
                }
            }
        }

        public String GetString() {
            return erg.toString();
        }
    }
}
