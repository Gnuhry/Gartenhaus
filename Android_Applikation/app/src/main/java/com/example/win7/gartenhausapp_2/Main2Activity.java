package com.example.win7.gartenhausapp_2;

import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.widget.TableLayout;
import android.widget.TableRow;
import android.widget.TextView;

public class Main2Activity extends AppCompatActivity {

    Client client;
    private String[]row_;
    private int index;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main2);
        SetDownMenu();
        client = MainActivity.client;
        FillTable();
        findViewById(R.id.imVaddPlant).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startActivity(new Intent(getApplicationContext(), Edit_Plant.class));
            }
        });
        findViewById(R.id.imVeditPlant).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent intent=new Intent(getApplicationContext(), Edit_Plant.class);
                intent.putExtra("ID",Integer.parseInt(row_[index].split("_")[0]));
                startActivity(intent);
            }
        });
    }

    private void SetDownMenu(){
        findViewById(R.id.downbar).setBackgroundResource(R.drawable.pflanze);
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
        String help = client.Send("get plant display");
        if (help.equals(getString(R.string.error))) {
            TableRow row = new TableRow(this);
            TextView txV = new TextView(this);
            txV.setText(getString(R.string.error));
            row.addView(txV);
            ((TableLayout) findViewById(R.id.tablePlant)).addView(row);
            return;
        }
        row_ = help.split(";");
        if (row_.length < 1) {
            return;
        }

        TableLayout tableLayout = findViewById(R.id.tablePlant);
        int counter=0;
        for (String aRow_ : row_) {
            TableRow row = new TableRow(this);
            TextView txV = new TextView(this);
            row.setTag(counter);
            txV.setText(aRow_.split("_")[0]);
            txV.setTag(counter);
            row.addView(txV);
            txV.setOnClickListener(new Edit());
            txV = new TextView(this);
            txV.setText(aRow_.split("_")[1]);
            txV.setTag(counter++);
            row.addView(txV);
            row.setOnClickListener(new Edit());
            txV.setOnClickListener(new Edit());
            tableLayout.addView(row);
        }
    }


    public class Edit implements View.OnClickListener { //Blau hintelegen, wenn angeklickt

        @Override
        public void onClick(View view) {
            findViewById(R.id.imVeditPlant).setVisibility(View.VISIBLE);
            TableLayout tableLayout = findViewById(R.id.tablePlant);
            for(int f=0;f<tableLayout.getChildCount();f++){
                tableLayout.getChildAt(f).setBackgroundColor(Color.TRANSPARENT);
            }
            tableLayout.getChildAt(Integer.parseInt(view.getTag().toString())+1).setBackgroundColor(Color.rgb(2,127,211));
            index=Integer.parseInt(view.getTag().toString());
        }
    }


}
