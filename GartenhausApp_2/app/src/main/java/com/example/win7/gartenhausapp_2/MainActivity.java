package com.example.win7.gartenhausapp_2;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.View;
import android.widget.TextView;
import android.widget.Toast;

public class MainActivity extends AppCompatActivity {

    private SharedPreferences.Editor editor;
    public static String IP = "192.168.178.78";
    public static final int Port = 5000;
    public static Client client;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        SetDownMenu();
        SharedPreferences sharedPreferences = getSharedPreferences("Speicher", MODE_PRIVATE);
        editor = sharedPreferences.edit();
        IP = sharedPreferences.getString("IP", IP);
        client = new Client(IP, Port);
        ((TextView) findViewById(R.id.edTIP)).setText(IP);
        ((TextView) findViewById(R.id.edTIP)).addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {

            }

            @Override
            public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

            }

            @Override
            public void afterTextChanged(Editable editable) {
                char[] test = editable.toString().toCharArray();
                int count = 0;
                for (char Test_ : test)
                    if (Test_ == '.')
                        count++;

                if (count == 3 && test.length < 16)  //check for ip4 format
                {
                    IP = editable.toString();
                    client = new Client(IP, Port);
                    Toast.makeText(getApplicationContext(), "Neue IP: " + IP, Toast.LENGTH_SHORT).show();
                    editor.putString("IP", IP).apply();

                }
            }
        });

    }
    private void SetDownMenu(){
        findViewById(R.id.downbar).setBackgroundResource(R.drawable.home);
        findViewById(R.id.imVPlant).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startActivity(new Intent(getApplicationContext(), Main2Activity.class));
            }
        });
        findViewById(R.id.imVHome).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startActivity(new Intent(getApplicationContext(), MainActivity.class));
            }
        });
        findViewById(R.id.imVRegler).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startActivity(new Intent(getApplicationContext(), Main3Activity.class));
            }
        });
    }

    public void imV_information(View view) {
        startActivity(new Intent(getApplication(),Credits.class));
    }
}
