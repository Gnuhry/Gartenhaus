package com.example.win7.gartenhausapp_2;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.ImageView;
import android.widget.TableLayout;
import android.widget.TableRow;
import android.widget.TextView;
import android.widget.Toast;

public class Main3Activity extends AppCompatActivity {
    Client client;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main3);
        client = MainActivity.client;
        FillTable();
        client.Stop();
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
     * Methode to fill the table
     */
    @SuppressLint("SetTextI18n")
    private void FillTable() {
        String data = client.Send("get arduino display");
        try {
            Thread.sleep(500);
        } catch (InterruptedException e) {
            Toast.makeText(this, "Nope", Toast.LENGTH_SHORT).show();
        }
        if(data.equals("Error"))return;
        String[]row_=data.split(";");
        if(row_.length<1)return;

        TableLayout tableLayout = findViewById(R.id.tableArduino);
        for (String aRow_ : row_) {
            TableRow row = new TableRow(this);
            TextView txV = new TextView(this);
            txV.setText(aRow_.split("_")[2]);
            row.addView(txV);
            txV = new TextView(this);
            txV.setText(aRow_.split("_")[1]);
            row.addView(txV);
            ImageView imageButton = new ImageView(this);
            imageButton.setImageResource(R.drawable.outline_edit_black_18dp);
            imageButton.setTag(Integer.parseInt(aRow_.split("_")[0]));
            imageButton.setOnClickListener(new Edit());
            row.addView(imageButton);
            tableLayout.addView(row);
        }
    }

    /**
     * Click Listener for Graph Button
     */
    public void btn_Graph(View view) {
        //Toast.makeText(this,"In Arbeit",Toast.LENGTH_SHORT).show();
        startActivity(new Intent(getApplicationContext(), Graph.class));
    }

    /**
     * Click Listener for Edit Button
     */
    public class Edit implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(getApplicationContext(), Edit_Arduino.class);
            intent.putExtra("ID", (int) view.getTag());
            startActivity(intent);

        }
    }
}
