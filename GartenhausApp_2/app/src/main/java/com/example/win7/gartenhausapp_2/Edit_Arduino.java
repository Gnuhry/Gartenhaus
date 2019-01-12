package com.example.win7.gartenhausapp_2;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
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
    private List<String> spinnerlist;

    /**
     * Create the menue house item in the right top corner
     */
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.menu, menu);
        return super.onCreateOptionsMenu(menu);
    }

    /**
     * Click Listener for menue item
     */
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
        SpinnerInitalisieren();
        if (ID < 1) return;
        //get arduino data
        StringBuilder help = new StringBuilder(client.Send("get arduino all_" + ID));
        try {
            Thread.sleep(500);
        } catch (InterruptedException e) {
            Toast.makeText(this, "Nope", Toast.LENGTH_SHORT).show();
        }
        //Log.e("hung",help.toString());
        if(help.toString().equals("Error")){
            return;
        }
        String[] data = help.toString().split("_");
        ((TextView) findViewById(R.id.txVID)).setText(ID + "");
        ((EditText) findViewById(R.id.edTIP)).setText(data[1]);
        ((Spinner) findViewById(R.id.spinner)).setSelection(Integer.parseInt(data[2]));
    }

    /**
     * Create the data of the spinner
     */
    private void SpinnerInitalisieren() {
        Spinner spinner = findViewById(R.id.spinner);
        spinnerlist = new ArrayList<>();
        spinnerlist.add(getString(R.string.no));
        String name = client.Send("get plant names");
        try {
            Thread.sleep(500);
        } catch (InterruptedException e) {
            Toast.makeText(this, "Nope", Toast.LENGTH_SHORT).show();
        }
        if(!name.equals("Error")){
            String[] names = name.split("_");
            Collections.addAll(spinnerlist, names);
        }

        ArrayAdapter<String> arrayAdapter = new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, spinnerlist);
        arrayAdapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(arrayAdapter);
    }

    /**
     * Click Methode of Delete Button
     */
    public void btnClick_DeleteArduino(View view) {
        client.Send("delete arduino_" + ID);
        Close();
    }

    /**
     * Click Method of Save Button
     */
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

    /**
     * Click Methode of Close Button
     */
    public void btnClick_CloseArduino(View view) {
        Close();
    }

    /**
     * Close the form
     * Start the Parent activity
     */
    private void Close() {
        client.Stop();
        Intent intent = new Intent(this, Main3Activity.class);
        startActivity(intent);
    }

    /**
     * Click Methode of Live Button
     */
    public void btnClick_btnLive(View view) {
    //    startActivity(new Intent(getApplicationContext(), Live.class).putExtra("ID", ID));
    Toast.makeText(this,"In Arbeit",Toast.LENGTH_SHORT).show();
    }
}
