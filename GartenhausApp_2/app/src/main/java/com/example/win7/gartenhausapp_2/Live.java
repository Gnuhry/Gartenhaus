package com.example.win7.gartenhausapp_2;

import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.Switch;
import android.widget.Toast;

public class Live extends AppCompatActivity {

    Server server;
    Boolean startstop;
    int ID;

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.menu, menu);
        return super.onCreateOptionsMenu(menu);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        startActivity(new Intent(getApplicationContext(), MainActivity.class));
        return super.onOptionsItemSelected(item);
    }

    boolean temp, light;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_live);
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
            ((Button) view).setText(R.string.stop);
            findViewById(R.id.btnExit).setVisibility(View.GONE);
        } else if (((Button) view).getText() == getString(R.string.stop)) {
            startstop = true;
            findViewById(R.id.swheater).setEnabled(false);
            findViewById(R.id.swsprayer).setEnabled(false);
            findViewById(R.id.swpump).setEnabled(false);
            findViewById(R.id.swlight).setEnabled(false);
            findViewById(R.id.swcooler).setEnabled(false);
            findViewById(R.id.swshutters).setEnabled(false);
            server.Stop();
            ((Button) view).setText(R.string.start);
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
