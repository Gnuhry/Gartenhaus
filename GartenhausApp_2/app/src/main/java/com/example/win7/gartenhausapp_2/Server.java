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
import java.io.PrintStream;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;

public class Server {
    ServerSocket serverSocket;
    TextView[] txV;
    Switch[] sw;
    boolean first, end;
    Live live;

    public Server(TextView[] txV, Switch[] sw, Live live) {
        this.live = live;
        this.sw = sw;
        this.txV = txV;
        first = true;
        new Thread(new ServerThread()).start();
    }

    public void Stop() {
        end = true;
    }

    public void Restart() {
        first = true;
        if (serverSocket != null) {
            if (serverSocket.isBound()) {
                return;
            }
        }
        new Thread(new ServerThread()).start();
    }

    private class ServerThread extends Thread {

        @Override
        public void run() {
            end = false;
            try {
                serverSocket = new ServerSocket(5000);
                while (true) {
                    Socket client = serverSocket.accept();
                    new SocketAnswer(client, end).start();
                }
            } catch (IOException ex) {
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
                PrintStream printStream = new PrintStream(outputStream);
                StringBuilder sb = new StringBuilder();
                while (!end) {
                    do {
                        sb.append(bufferedReader.readLine());
                    } while (sb.indexOf("|") < 0);
                    Run x = new Run(sb.toString().split("_"));
                    Log.e("Server2", "Get: " + sb.toString());
                    live.runOnUiThread(x);
                    try {
                        Thread.sleep(500);
                    } catch (InterruptedException e) {
                        Toast.makeText(live, "Nope", Toast.LENGTH_SHORT).show();
                    }
                    printWriter.print(x.GetString() + "|");
                    printStream.print(x.GetString() + "|");
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

        public Run(String split[]) {
            this.split = split;
        }

        @Override
        public void run() {
            ((TextView) live.findViewById(R.id.txVtemp)).setText(split[0] + "°C");//txV[0].setText(split[0] + "°C");
            ((TextView) live.findViewById(R.id.txVhumid)).setText(split[1]);//txV[1].setText(split[1] );
            ((TextView) live.findViewById(R.id.txVgroundHumid)).setText(split[2]);//txV[2].setText(split[2] );
            ((TextView) live.findViewById(R.id.txVlight)).setText(split[3]);//txV[3].setText(split[3] );
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
