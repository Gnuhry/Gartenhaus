package com.example.win7.gartenhausapp_2;

import android.content.Context;
import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.ArrayAdapter;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.Toast;

import java.util.ArrayList;
import java.util.List;

public class Edit_Plant extends AppCompatActivity {


    private Client client;
    private int ID;
    private List<String> spinnerlist;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_edit__plant);
        client = MainActivity.client;
        ID = getIntent().getIntExtra("ID", -1); //read ID
        InitClickListner();
        SpinnerInitalisieren();
        if (ID <1)
            return;
        findViewById(R.id.imVDeletePlaint).setVisibility(View.VISIBLE);
        String[] help = client.Send("get plant all_" + ID).split("_");
        try {
            Thread.sleep(500);
        } catch (InterruptedException e) {
            Toast.makeText(this, "Nope", Toast.LENGTH_SHORT).show();
        }
        if(help[0].equals("Error")){
            return;
        }
        if (help.length < 2) return;
        ((EditText) findViewById(R.id.edTName)).setText(help[1]);
        ((EditText) findViewById(R.id.edTminTemp)).setText(help[2].replace(',', '.'));
        ((EditText) findViewById(R.id.edTmaxTemp)).setText(help[3].replace(',', '.'));
        ((EditText) findViewById(R.id.edTminGroundHumid)).setText(help[4].replace(',', '.'));
        ((EditText) findViewById(R.id.edTmaxGroundHumid)).setText(help[5].replace(',', '.'));
        ((EditText) findViewById(R.id.edTminHumid)).setText(help[6].replace(',', '.'));
        ((EditText) findViewById(R.id.edTmaxHumid)).setText(help[7].replace(',', '.'));
        ((Spinner) findViewById(R.id.spinnerLight)).setSelection(Integer.parseInt(help[8]));
    }

    private void SpinnerInitalisieren() {
        Spinner spinner = findViewById(R.id.spinnerLight);
        spinnerlist = new ArrayList<>();
        spinnerlist.add("sehr empfindlich");
        spinnerlist.add("empfindlich");
        spinnerlist.add("grob empfindlich");
        ArrayAdapter<String> arrayAdapter = new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, spinnerlist);
        arrayAdapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(arrayAdapter);
        spinner.setSelection(1);
    }

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

    /**
     * Create the ClickListner
     */
    private void InitClickListner() {
        findViewById(R.id.imVSavePlaint).setOnClickListener(new View.OnClickListener() {
            /**
             * Click Listner for Save Button
             */
            @Override
            public void onClick(View view) {
                String name = ((EditText) findViewById(R.id.edTName)).getText().toString();
                String MinTemp = ((EditText) findViewById(R.id.edTminTemp)).getText().toString();
                String MaxTemp = ((EditText) findViewById(R.id.edTmaxTemp)).getText().toString();
                String MinGroundHumid = ((EditText) findViewById(R.id.edTminGroundHumid)).getText().toString();
                String MaxGroundHumid = ((EditText) findViewById(R.id.edTmaxGroundHumid)).getText().toString();
                String MinHumid = ((EditText) findViewById(R.id.edTminHumid)).getText().toString();
                String MaxHumid = ((EditText) findViewById(R.id.edTmaxHumid)).getText().toString();

                if (name.trim().equals("") || MinTemp.equals("") || MaxTemp.equals("") || MinGroundHumid.equals("") ||
                        MaxGroundHumid.equals("") || MinHumid.equals("") || MaxHumid.equals("")) {
                    Toast.makeText(getApplicationContext(), getString(R.string.edit_all), Toast.LENGTH_LONG).show();
                    return;
                }
                if (Float.parseFloat(MinTemp) > 50 || Float.parseFloat(MinTemp) < 0 || Float.parseFloat(MaxTemp) > 50 || Float.parseFloat(MinTemp) < 0) {
                    Toast.makeText(getApplicationContext(), R.string.tempValue, Toast.LENGTH_LONG).show();
                    return;
                }
                if (Float.parseFloat(MinHumid) > 100 || Float.parseFloat(MinHumid) < 0 || Float.parseFloat(MaxHumid) > 100 || Float.parseFloat(MaxHumid) < 0) {
                    Toast.makeText(getApplicationContext(), R.string.humidValue, Toast.LENGTH_LONG).show();
                    return;
                }
                if (Float.parseFloat(MinGroundHumid) > 100 || Float.parseFloat(MinGroundHumid) < 0 || Float.parseFloat(MaxGroundHumid) > 100 || Float.parseFloat(MaxGroundHumid) < 0) {
                    Toast.makeText(getApplicationContext(), R.string.humidValue, Toast.LENGTH_LONG).show();
                    return;
                }
                if (Float.parseFloat(MinTemp) > Float.parseFloat(MaxTemp)) {
                    Toast.makeText(getApplicationContext(), R.string.minGmaxTemp, Toast.LENGTH_LONG).show();
                    return;
                }
                if (Float.parseFloat(MinHumid) > Float.parseFloat(MaxHumid)) {
                    Toast.makeText(getApplicationContext(), getString(R.string.minGmaxHumid), Toast.LENGTH_LONG).show();
                    return;
                }
                if (Float.parseFloat(MinGroundHumid) > Float.parseFloat(MaxGroundHumid)) {
                    Toast.makeText(getApplicationContext(), getString(R.string.minGmaxGroundHumid), Toast.LENGTH_LONG).show();
                    return;
                }
                if (ID != -1) {
                    Log.e("Change", client.Send("set plant_" + ID + "_" + name + "_" + MinTemp.replace('.', ',') +
                            "_" + MaxTemp.replace('.', ',') + "_" + MinGroundHumid.replace('.', ',') + "_" + MaxGroundHumid.replace('.', ',') +
                            "_" + MinHumid.replace('.', ',') + "_" + MaxHumid.replace('.', ',') +
                            "_"+((Spinner) findViewById(R.id.spinnerLight)).getSelectedItemPosition()));
                    try {
                        Thread.sleep(500);
                    } catch (InterruptedException e) {
                        Toast.makeText(getApplicationContext(), "Nope", Toast.LENGTH_SHORT).show();
                    }
                } else {
                    Log.e("Add", client.Send("new plant_" + name + "_" + MinTemp.replace('.', ',') + "_" + MaxTemp.replace('.', ',') + "_" + MinGroundHumid.replace('.', ',') +
                            "_" + MaxGroundHumid.replace('.', ',') + "_" + MinHumid.replace('.', ',') + "_" + MaxHumid.replace('.', ',') +
                            "_" + ((Spinner) findViewById(R.id.spinnerLight)).getSelectedItemPosition()  ));
                }
                Close();
            }
        });
        findViewById(R.id.imVClosePlaint).setOnClickListener(new View.OnClickListener() {
            /**
             * Click Listener for Close Button
             */
            @Override
            public void onClick(View view) {
                Close();
            }
        });
        if (ID != -1)
            findViewById(R.id.imVDeletePlaint).setOnClickListener(new View.OnClickListener() {
                /**
                 * Click Listener for Delete Button
                 */
                @Override
                public void onClick(View view) {
                    Log.e("Delete", client.Send("delete arduino_" + ID));
                    Close();
                }
            });
    }

    /**
     * Close the form
     */
    private void Close() {
        client.Stop();
        Intent intent = new Intent(this, Main2Activity.class);
        startActivity(intent);
    }

    /**
     * Click Listener for Clicking the Form to close the keyboard
     */
    public void btnClick_Tastatur(View view) {
        InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
        try {
            if (imm != null) {
                imm.hideSoftInputFromWindow(getCurrentFocus().getWindowToken(), 0);
            }
        } catch (NullPointerException e) {
            e.printStackTrace();
        }

    }
}
