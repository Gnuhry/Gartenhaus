package com.example.win7.gartenhausapp_2;

import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Spinner;
import android.widget.TableLayout;
import android.widget.TableRow;
import android.widget.TextView;
import android.widget.Toast;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class Main3Activity extends AppCompatActivity {
    Client client;
    private String[]row_;
    private int index;
    List<String>spinnerlist;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main3);
        SetDownMenu();
        client = MainActivity.client;
        FillTable();
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
    private void FillTable() {
        TableLayout tableLayout = findViewById(R.id.tableArduino);
        String data = client.Send("get arduino display");

        if (data.equals(getString(R.string.error))) {
            TableRow row = new TableRow(this);
            TextView txV = new TextView(this);
            txV.setText(getString(R.string.error));
            row.addView(txV);
            ((TableLayout) findViewById(R.id.tableArduino)).addView(row);
            return;
        }
        row_ = data.split(";");
        if (row_.length < 1) {
            return;
        }

        int counter=0;
        for (String aRow_ : row_) {
            TableRow row = new TableRow(this);
            TextView txV = new TextView(this);
            row.setTag(counter);
            txV.setText(aRow_.split("_")[2]);
            txV.setTag(counter);
            txV.setOnClickListener(new Edit());
            row.addView(txV);
            txV = new TextView(this);
            txV.setText(aRow_.split("_")[1]);
            txV.setOnClickListener(new Edit());
            txV.setTag(counter++);
            row.addView(txV);
            row.setOnClickListener(new Edit());
            tableLayout.addView(row);
        }
    }


    private void SpinnerInitalisieren() {
        Spinner spinner = findViewById(R.id.spinner);
         spinnerlist = new ArrayList<>();
        spinnerlist.add(getString(R.string.no));
        String name = client.Send("get plant names");
        if (!name.equals(getString(R.string.error))) {
            String[] names = name.split("_");
            Collections.addAll(spinnerlist, names);
        } else {
            spinnerlist.clear();
            spinnerlist.add(getString(R.string.error));
            spinner.setEnabled(false);
        }

        ArrayAdapter<String> arrayAdapter = new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, spinnerlist);
        arrayAdapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(arrayAdapter);
    }


    private void Reload() {
        startActivity(getIntent());
    }

    public void btnClick_SaveArduino(View view) {
        client.Send("set arduino plantid_" + row_[index].split("_")[0] + "_" + (((Spinner) findViewById(R.id.spinner)).getSelectedItemPosition()));
        try {
            Thread.sleep(1900);
        } catch (InterruptedException e) {
            Toast.makeText(this, "Nope", Toast.LENGTH_SHORT).show();
        }
        Reload();
    }
    public void btn_Graph(View view) {
        startActivity(new Intent(getApplicationContext(), Graph.class));
    }

    public void remote_control(View view) {
        startActivity(new Intent(getApplicationContext(), Live.class).putExtra("ID", Integer.parseInt(row_[index].split("_")[0])));
    }

    public class Edit implements View.OnClickListener { //Blau hinterlegen wenn anklicken und Bearbeiten ermöglichen
        @Override
        public void onClick(View view) {
            TableLayout tableLayout = findViewById(R.id.tableArduino);
            for(int f=0;f<tableLayout.getChildCount();f++){
                tableLayout.getChildAt(f).setBackgroundColor(Color.TRANSPARENT);
            }
            tableLayout.getChildAt(Integer.parseInt(view.getTag().toString())+1).setBackgroundColor(Color.rgb(2,127,211));

            if(findViewById(R.id.imVSaveArduino).getVisibility()!=View.VISIBLE)
            {
                SpinnerInitalisieren();
            }
            findViewById(R.id.imVSaveArduino).setVisibility(View.VISIBLE);
            findViewById(R.id.ll).setVisibility(View.VISIBLE);
            findViewById(R.id.imVremote_control).setVisibility(View.VISIBLE);
           index=Integer.parseInt(view.getTag().toString());
            Spinner spinner = findViewById(R.id.spinner);
            spinner.setSelection(spinnerlist.indexOf(row_[index].split("_")[1]));

        }
    }
}
