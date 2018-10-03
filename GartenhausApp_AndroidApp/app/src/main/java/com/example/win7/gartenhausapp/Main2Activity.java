package com.example.win7.gartenhausapp;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.ImageView;
import android.widget.TableLayout;
import android.widget.TableRow;
import android.widget.TextView;
import android.widget.Toast;

public class Main2Activity extends AppCompatActivity {

    Client client;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main2);
        client=MainActivity.client; //Client bekommen von der Haupt Activity
     //   Toast.makeText(this,"yeh",Toast.LENGTH_SHORT).show();
//        client.Send("get time");
          TabelleFullen();
        client.Stop(); //client stoppen
        findViewById(R.id.imageView).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) { //Click Listener
                startActivity(new Intent(getApplicationContext(),Edit_Plant.class));
            }
        });
    }
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


    @SuppressLint("SetTextI18n")//Ausblenden der Fehlermeldung: "Kein festen Text in TextViews ausgeben"
    private void TabelleFullen() { //Tabelle füllen
        String help=client.Send("get IDS");
        if(help=="Error")return;
        String[] ID=help.split("_"); //IDS bekommen vom Client(Server)
        try {
            Thread.sleep(500); //warten auf Antwort
        } catch (InterruptedException e) {
            Toast.makeText(this,"Nope",Toast.LENGTH_SHORT).show();
        }
        Log.e("Länge",""+ID.length);
        if(ID.length<=1){
            Toast.makeText(getApplicationContext(),"IDS",Toast.LENGTH_SHORT).show();
            return;}

//        Tabele mit ID, Name , Platz und BEarbeitung erstellen
        TableLayout tableLayout=findViewById(R.id.tablePlant);
        for (String ID_ : ID) {
            TableRow row = new TableRow(this);
            TextView txV = new TextView(this);
            txV.setText(ID_ + "");
            row.addView(txV);
            txV = new TextView(this);
            txV.setText(client.Send("get name_" + ID_));
            try {
                Thread.sleep(500);
            } catch (InterruptedException e) {
                Toast.makeText(this,"Nope",Toast.LENGTH_SHORT).show();
            }
            row.addView(txV);
            txV = new TextView(this);
            txV.setText(ID_ + "");
            row.addView(txV);
            ImageView imageButton = new ImageView(this);
            imageButton.setImageResource(R.drawable.outline_edit_black_18dp);
            imageButton.setTag(Integer.parseInt(ID_));
            imageButton.setOnClickListener(new Edit());
            row.addView(imageButton);
            tableLayout.addView(row);
        }

    }

    public class Edit implements View.OnClickListener{

        @Override
        public void onClick(View view) { //Bearbeitung Activity starten
            Intent intent = new Intent(getApplicationContext(),Edit_Plant.class);
            intent.putExtra("ID",(int)view.getTag()); //ID mitsenden
            startActivity(intent);
           // Toast.makeText(getApplicationContext(),"HEy"+view.getTag(),Toast.LENGTH_LONG).show();
        }
    }

}
