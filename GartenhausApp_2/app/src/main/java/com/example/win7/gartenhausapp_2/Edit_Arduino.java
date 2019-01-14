package com.example.win7.gartenhausapp_2;

import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class Edit_Arduino extends AppCompatActivity {

    private Client client;
    private int ID;

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

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_edit__arduino);
        client = MainActivity.client;
        ID = getIntent().getIntExtra("ID", -1); //read ID
        if (ID < 1) return;
        StringBuilder help = new StringBuilder(client.Send("get arduino all_" + ID));
        if (help.toString().equals(getString(R.string.error))) {
            ((TextView) findViewById(R.id.txVID)).setText(R.string.error);
            findViewById(R.id.edTIP).setEnabled(false);
            findViewById(R.id.spinner).setEnabled(false);
            findViewById(R.id.imVSaveArduino).setVisibility(View.GONE);
            return;
        }
        SpinnerInitalisieren();
        String[] data = help.toString().split("_");
        ((TextView) findViewById(R.id.txVID)).setText(String.valueOf(ID));
        ((EditText) findViewById(R.id.edTIP)).setText(data[1]);
        ((Spinner) findViewById(R.id.spinner)).setSelection(Integer.parseInt(data[2]));
    }

    private void SpinnerInitalisieren() {
        Spinner spinner = findViewById(R.id.spinner);
        List<String> spinnerlist = new ArrayList<>();
        spinnerlist.add(getString(R.string.no));
        String name = client.Send("get plant names");
        if (!name.equals(getString(R.string.error))) {
            String[] names = name.split("_");
            Collections.addAll(spinnerlist, names);
        } else {
            spinnerlist.add(getString(R.string.error));
            spinner.setEnabled(false);
        }

        ArrayAdapter<String> arrayAdapter = new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, spinnerlist);
        arrayAdapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(arrayAdapter);
    }

    public void btnClick_DeleteArduino(View view) {
        client.Send("delete arduino_" + ID);
        Close();
    }

    public void btnClick_SaveArduino(View view) {
        client.Send("set arduino_" + ID + "_" + ((EditText) findViewById(R.id.edTIP)).getText().toString() +
                "_" + (((Spinner) findViewById(R.id.spinner)).getSelectedItemPosition()));
        try {
            Thread.sleep(1900);
        } catch (InterruptedException e) {
            Toast.makeText(this, "Nope", Toast.LENGTH_SHORT).show();
        }
        Close();
    }

    private void Close() {
        Intent intent = new Intent(this, Main3Activity.class);
        startActivity(intent);
    }

    public void btnClick_btnLive(View view) {
        startActivity(new Intent(getApplicationContext(), Live.class).putExtra("ID", ID));
    }
}
