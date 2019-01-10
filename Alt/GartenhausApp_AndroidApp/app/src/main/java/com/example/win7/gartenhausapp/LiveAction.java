package com.example.win7.gartenhausapp;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.support.v7.app.AppCompatActivity;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.CompoundButton;
import android.widget.Switch;
import android.widget.TextView;
import android.widget.Toast;

public class LiveAction extends AppCompatActivity {

    private Client clientArduino;
    private int ID;
    private Handler handler;
    private myThread myThread;

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.menu,menu); //Menü initalisieren
        return super.onCreateOptionsMenu(menu);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        startActivity(new Intent(getApplicationContext(),MainActivity.class)); //Menü Click Listener
        return super.onOptionsItemSelected(item);
    }


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_live_action);
        ID = getIntent().getIntExtra("ID", -1);
        if(ID <0)startActivity(new Intent(getApplicationContext(),Main3Activity.class));
        String ip = MainActivity.client.Send("get arduino arduinoIP_" + ID);
        try {
            Thread.sleep(500);
        } catch (InterruptedException e) {
            Toast.makeText(this, "Nope", Toast.LENGTH_SHORT).show();
        }
        //TODO Fehler: Sendet an Server anstatt an Arduino!!
        clientArduino = new Client(ip, 5001);
      //  Starten();
        String on[] = clientArduino.Send("on").split("_");
        if (on[0].equals("y")) ((Switch) findViewById(R.id.swHeizung)).setChecked(true);
        if (on[1].equals("y")) ((Switch) findViewById(R.id.swKuhlung)).setChecked(true);
        if (on[2].equals("y")) ((Switch) findViewById(R.id.swPumpe)).setChecked(true);
        if (on[3].equals("y")) ((Switch) findViewById(R.id.swSpruher)).setChecked(true);
        if (on[4].equals("y")) ((Switch) findViewById(R.id.swLicht)).setChecked(true);
        if (on[5].equals("y")) ((Switch) findViewById(R.id.swRolladen)).setChecked(true);
            ((Switch) findViewById(R.id.swHeizung)).setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
                @Override
                public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                    clientArduino.Send("0_"+b);
                    try {
                        Thread.sleep(500);
                    } catch (InterruptedException e) {
                        e.printStackTrace();
                    }
                }
            });
        ((Switch) findViewById(R.id.swKuhlung)).setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                clientArduino.Send("1_"+b);
                try {
                    Thread.sleep(500);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        });
        ((Switch) findViewById(R.id.swPumpe)).setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                clientArduino.Send("2_"+b);
                try {
                    Thread.sleep(500);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        });
        ((Switch) findViewById(R.id.swSpruher)).setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                clientArduino.Send("3_"+b);
                try {
                    Thread.sleep(500);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        });
        ((Switch) findViewById(R.id.swLicht)).setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                clientArduino.Send("4_"+b);
                try {
                    Thread.sleep(500);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        });
        ((Switch) findViewById(R.id.swRolladen)).setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                clientArduino.Send("5_"+b);
                try {
                    Thread.sleep(500);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        });
    }

    private void Starten() {
        handler.postDelayed(myThread,5000);
    }

    public void btnClick_Close(View view) {
        startActivity(new Intent(getApplicationContext(),Edit_Arduino.class).putExtra("ID",ID));
    }
}
class myThread implements Runnable{

    private Activity activity;
    private Client client;
    private int ID;
    myThread(Activity activity, Client client,int ID){
        this.ID=ID;
        this.activity=activity;
        this.client=client;
    }
    @Override
    public void run() {
        try {
            wait(5000);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
        String help=client.Send("get arduino data");
        try {
            Thread.sleep(500);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
        String help2[]=help.split("|");
        String data[]=help2[help2.length-1].split("_");
        ((TextView)activity.findViewById(R.id.txVTemp)).setText("Temperatur: "+data[1]+" °C");
        ((TextView)activity.findViewById(R.id.txVFeucht)).setText("Feuchtigkeit: "+data[2]);
        ((TextView)activity.findViewById(R.id.txVHumid)).setText("Humid: "+data[3]);
        ((TextView)activity.findViewById(R.id.txVUV)).setText("UV: "+data[4]);
        //Starten();
    }
    }

