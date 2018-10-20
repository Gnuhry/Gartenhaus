package com.example.win7.gartenhausapp;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.ImageView;
import android.widget.TableLayout;
import android.widget.TableRow;
import android.widget.TextView;
import android.widget.Toast;

public class Main3Activity extends AppCompatActivity {

    Client client;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main3);
        client=MainActivity.client;
        TabelleFullen();
        client.Stop();
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

    @SuppressLint("SetTextI18n")
    private void TabelleFullen() {
        String [] ID =client.Send("get arduinoids").split("_");
        try {
            Thread.sleep(500); //warten auf Antwort
        } catch (InterruptedException e) {
            Toast.makeText(this,"Nope",Toast.LENGTH_SHORT).show();
        }
        Log.e("Länge",""+ID.length);
        if(ID.length<=1){
            Toast.makeText(getApplicationContext(),"IDS",Toast.LENGTH_SHORT).show();
            return;}
        TableLayout tableLayout=findViewById(R.id.tableArduino);
        for(String ID_:ID){ //Tabelle füllen
            TableRow row = new TableRow(this);
            TextView txV = new TextView(this);
            txV.setText(ID_ + "");
            row.addView(txV);
            txV = new TextView(this);
            String help=client.Send("get arduino all" + ID_);
            try {
                Thread.sleep(500);
            } catch (InterruptedException e) {
                Toast.makeText(this,"Nope",Toast.LENGTH_SHORT).show();
            }
            txV.setText(help.split("_")[1]);
            row.addView(txV);
            ImageView imageButton = new ImageView(this);
            imageButton.setImageResource(R.drawable.outline_edit_black_18dp);
            imageButton.setTag(Integer.parseInt(ID_));
            imageButton.setOnClickListener(new Main3Activity.Edit());
            row.addView(imageButton);
            tableLayout.addView(row);
        }

    }

    public void btnClick_btnLive(View view) {
        Intent intent = new Intent(getApplicationContext(),LiveAction.class);
        intent.putExtra("ID",(int)view.getTag()); //ID mitsenden
        startActivity(intent);
    }

    public void btn_Graphen(View view) {
        startActivity(new Intent(getApplicationContext(),Graphen.class));
    }

    public class Edit implements View.OnClickListener {
        @Override
        public void onClick(View view) {//Bearbeitung Activity starten
            Intent intent = new Intent(getApplicationContext(),Edit_Arduino.class);
            intent.putExtra("ID",(int)view.getTag()); //ID mitsenden
            startActivity(intent);
            // Toast.makeText(getApplicationContext(),"HEy"+view.getTag(),Toast.LENGTH_LONG).show();

        }
    }
}
