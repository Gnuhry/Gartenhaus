package com.example.win7.gartenhausapp;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import java.util.ArrayList;
import java.util.List;

public class Edit_Arduino extends AppCompatActivity {

    private Client client;
    private int ID;
    private List<String> spinnerlist;
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
        setContentView(R.layout.activity_edit__arduino);
        client=MainActivity.client;
        ID=getIntent().getIntExtra("ID",-1); //mitgesendete ID auslesen
        SpinnerInitalisieren();
         if(ID==-1) return; //Wenn neu dann return
       // findViewById(R.id.imVDeletePlaint).setVisibility(View.VISIBLE);
        ((TextView)findViewById(R.id.txVID)).setText(ID+"");
        ((EditText)findViewById(R.id.edTIP)).setText(client.Send("get arduinoip_"+ID));
        try {
            Thread.sleep(500);
        } catch (InterruptedException e) {
            Toast.makeText(this,"Nope",Toast.LENGTH_SHORT).show();
        }
        StringBuilder help= new StringBuilder(client.Send("get arduino data_"+ID));
        try {
            Thread.sleep(500);
        } catch (InterruptedException e) {
            Toast.makeText(this,"Nope",Toast.LENGTH_SHORT).show();
        }
        String erg[]= help.toString().split("|");
        help = new StringBuilder();
        for(String x : erg)
            help.append(x).append("\n");
        ((TextView)findViewById(R.id.txVdataSend)).setText(help.toString());
        String h=client.Send("get arduinoidpflanze_"+ID);
        try {
            Thread.sleep(500);
        } catch (InterruptedException e) {
            Toast.makeText(this,"Nope",Toast.LENGTH_SHORT).show();
        }
        String name=client.Send("get name_"+h);
        try {
            Thread.sleep(500);
        } catch (InterruptedException e) {
            Toast.makeText(this,"Nope",Toast.LENGTH_SHORT).show();
        }
        ((Spinner)findViewById(R.id.spinner)).setSelection(spinnerlist.indexOf(name));
    }

    private void SpinnerInitalisieren() {
        Spinner spinner= findViewById(R.id.spinner);
         spinnerlist= new ArrayList<>();
        String[] ID=client.Send("get IDS").split("_"); //IDS bekommen vom Client(Server)
        try {
            Thread.sleep(500); //warten auf Antwort
        } catch (InterruptedException e) {
            Toast.makeText(this,"Nope",Toast.LENGTH_SHORT).show();
        }
        for(String ID_:ID){
            spinnerlist.add(client.Send("get name_"+ID_));
            try {
                Thread.sleep(500); //warten auf Antwort
            } catch (InterruptedException e) {
                Toast.makeText(this,"Nope",Toast.LENGTH_SHORT).show();
            }
        }
        ArrayAdapter<String> arrayAdapter= new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, spinnerlist);
        arrayAdapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(arrayAdapter);
    }

    public void btnClick_DeleteArduino(View view) {
        client.Send("delete arduino_"+ID);
        Close();
    }

    public void btnClick_SaveArduino(View view) {
        String ip=((EditText)findViewById(R.id.edTIP)).getText().toString();
        client.Send("set arduinoip_"+ID+"_"+ip);
        Close();
    }

    public void btnClick_CloseArduino(View view) {
        Close();
    }
    private void Close() {//Schließen
        client.Stop();//stoppen vom client
        Intent intent = new Intent(this,Main2Activity.class); //Main2Activity starten
        startActivity(intent);
    }

    public void btnClick_btnLive(View view) {
        startActivity(new Intent(getApplicationContext(),LiveAction.class).putExtra("ID",ID));
    }
}
