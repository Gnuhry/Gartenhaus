package com.example.win7.gartenhausapp_2;

import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.ImageView;
import android.widget.Switch;
import android.widget.Toast;

public class Live extends AppCompatActivity {

    Server server;
    Boolean startstop;
    int ID;
    boolean temp;

    @Override
    public void onBackPressed() { //Zurück Taste Abfangen, um Server zu stoppen
        if(server!=null){
        server.Stop();}
        super.onBackPressed();
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_live);
        SetDownMenu();
        ID = getIntent().getIntExtra("ID", -1); //read ID
        startstop = true;
        temp = true;
        server = new Server(new Switch[]{findViewById(R.id.swheater), findViewById(R.id.swsprayer), findViewById(R.id.swpump), findViewById(R.id.swlight), findViewById(R.id.swcooler), findViewById(R.id.swshutters)}, this);
        ((Switch) findViewById(R.id.swheater)).setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                if (!temp) {
                    return;
                }
                temp = false;
                if (!((Switch) findViewById(R.id.swheater)).isChecked()) {
                    ((Switch) findViewById(R.id.swheater)).setChecked(false);
                } else {
                    ((Switch) findViewById(R.id.swcooler)).setChecked(false);
                    ((Switch) findViewById(R.id.swheater)).setChecked(true);
                }
                temp = true;
            }
        });
        ((Switch) findViewById(R.id.swcooler)).setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                if (!temp) {
                    return;
                }
                temp = false;
                if (!((Switch) findViewById(R.id.swcooler)).isChecked()) {
                    ((Switch) findViewById(R.id.swcooler)).setChecked(false);

                } else {
                    ((Switch) findViewById(R.id.swcooler)).setChecked(true);
                    ((Switch) findViewById(R.id.swheater)).setChecked(false);
                }
                temp = true;
            }
        });
        ((Switch) findViewById(R.id.swlight)).setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                if (!temp) {
                    return;
                }
                temp = false;
                if (!((Switch) findViewById(R.id.swlight)).isChecked()) {
                    ((Switch) findViewById(R.id.swlight)).setChecked(false);
                } else {
                    ((Switch) findViewById(R.id.swshutters)).setChecked(false);
                    ((Switch) findViewById(R.id.swlight)).setChecked(true);
                }
                temp = true;
            }
        });
        ((Switch) findViewById(R.id.swshutters)).setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                if (!temp) {
                    return;
                }
                temp = false;
                if (!((Switch) findViewById(R.id.swshutters)).isChecked()) {
                    ((Switch) findViewById(R.id.swshutters)).setChecked(false);
                } else {
                    ((Switch) findViewById(R.id.swshutters)).setChecked(true);
                    ((Switch) findViewById(R.id.swlight)).setChecked(false);
                }
                temp = true;
            }
        });

    }

    private void SetDownMenu(){
        findViewById(R.id.downbar).setBackgroundResource(R.drawable.controller);
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

    public void btnStartStop(View view) {
        if (startstop) {
            startstop = false;
            findViewById(R.id.swheater).setEnabled(true);
            findViewById(R.id.swsprayer).setEnabled(true);
            findViewById(R.id.swpump).setEnabled(true);
            findViewById(R.id.swlight).setEnabled(true);
            findViewById(R.id.swcooler).setEnabled(true);
            findViewById(R.id.swshutters).setEnabled(true);
            server.Restart();
            MainActivity.client.Send("live_" + ID);
            ((ImageView) view).setImageResource(R.drawable.power_button_green);
            view.setTag("Stop");
        } else if (view.getTag() == "Stop") {
            startstop = true;
            ((ImageView) view).setImageResource(R.drawable.power_button_red);
            findViewById(R.id.swheater).setEnabled(false);
            findViewById(R.id.swsprayer).setEnabled(false);
            findViewById(R.id.swpump).setEnabled(false);
            findViewById(R.id.swlight).setEnabled(false);
            findViewById(R.id.swcooler).setEnabled(false);
            findViewById(R.id.swshutters).setEnabled(false);
            server.Stop();
            Intent intent = new Intent(this, Main3Activity.class);
            startActivity(intent);
        } else {
            Toast.makeText(this, R.string.please_wait, Toast.LENGTH_LONG).show();
        }
    }

    public void btnClick_btnExit(View view) {
        Intent intent = new Intent(this, Main3Activity.class);
        startActivity(intent);
    }
}
