package com.example.win7.gartenhausapp;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.TextView;
import android.widget.Toast;

public class MainActivity extends AppCompatActivity {

    private SharedPreferences.Editor editor; //SpeicherEditor initalisieren
    public static String IP="192.168.178.78"; //IP des Server initalisieren
    public static Client client; //client initalisieren

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
        setContentView(R.layout.activity_main);
        SharedPreferences sharedPreferences = getSharedPreferences("Speicher", MODE_PRIVATE); //Speicher initalisieren
        editor= sharedPreferences.edit(); //Editor initalisieren
        IP= sharedPreferences.getString("IP",IP); //IP aus Speicher auslesen
        client=new Client(IP); //Client initalisiern mit IP
        ((TextView)findViewById(R.id.edTIP)).setText(IP); //IP ausgeben
        ((TextView)findViewById(R.id.edTIP)).addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {

            }

            @Override
            public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

            }

            @Override
            public void afterTextChanged(Editable editable) { //Wenn Text sich ändert
                char[] test=editable.toString().toCharArray();
                int count=0;
                for (char Test_ : test)
                    if (Test_ == '.')
                        count++;

               // Toast.makeText(getApplicationContext(),test+","+test.split(".").length,Toast.LENGTH_SHORT).show();
                if(count==3&&test.length<16) //Wenn nicht IP4 Format
                {
                    IP=editable.toString();
                    client=new Client(IP); //neuen Client erstellen
                Toast.makeText(getApplicationContext(),"Neue IP: "+IP,Toast.LENGTH_SHORT).show();
                editor.putString("IP",IP).apply(); //IP speichern

                }
            }
        });

    }


    public void btnClick2(View view) {
        Intent intent = new Intent(this,Main2Activity.class); //Activity Main2Actovity starten
        startActivity(intent);

    }


    public void btnClick3(View view) {
        Intent intent = new Intent(this,Graphen.class); //Activity Main2Actovity starten
        startActivity(intent);
    }
}

