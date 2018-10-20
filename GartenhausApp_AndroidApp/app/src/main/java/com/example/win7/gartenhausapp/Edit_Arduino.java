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

        StringBuilder help= new StringBuilder(client.Send("get arduino all_"+ID));
        try {
            Thread.sleep(500);
        } catch (InterruptedException e) {
            Toast.makeText(this,"Nope",Toast.LENGTH_SHORT).show();
        }
        ((TextView)findViewById(R.id.txVID)).setText(ID+"");
        ((EditText)findViewById(R.id.edTIP)).setText(help.toString().split("_")[0]);
        String erg[]= help.toString().split("_")[2].split("|");
        StringBuilder help2 = new StringBuilder();
        for(String x : erg)
            help2.append(x).append("\n");
        ((TextView)findViewById(R.id.txVdataSend)).setText(help2.toString());
        ((Spinner)findViewById(R.id.spinner)).setSelection(spinnerlist.indexOf(help.toString().split("_")[1]));
    }

    private void SpinnerInitalisieren() {
        Spinner spinner= findViewById(R.id.spinner);
         spinnerlist= new ArrayList<>();
        String[] names=client.Send("get plant names").split("_"); //IDS bekommen vom Client(Server)
        try {
            Thread.sleep(500); //warten auf Antwort
        } catch (InterruptedException e) {
            Toast.makeText(this,"Nope",Toast.LENGTH_SHORT).show();
        }
        for(String name:names){
            spinnerlist.add(name);
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
        client.Send("set arduino _"+ID+"_"+((EditText)findViewById(R.id.edTIP)).getText().toString()+
                "_"+((Spinner)findViewById(R.id.spinner)).getSelectedItemPosition());
        try {
            Thread.sleep(500); //warten auf Antwort
        } catch (InterruptedException e) {
            Toast.makeText(this,"Nope",Toast.LENGTH_SHORT).show();
        }
        Close();
    }

    public void btnClick_CloseArduino(View view) {
        Close();
    }
    private void Close() {//Schließen
        client.Stop();//stoppen vom client
        Intent intent = new Intent(this,Main3Activity.class); //Main2Activity starten
        startActivity(intent);
    }

    public void btnClick_btnLive(View view) {
        startActivity(new Intent(getApplicationContext(),LiveAction.class).putExtra("ID",ID));
    }
}
