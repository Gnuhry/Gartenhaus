package com.example.win7.gartenhausapp_2;

import android.content.Intent;
import android.graphics.Color;
import android.graphics.Paint;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.SeekBar;
import android.widget.Spinner;
import android.widget.Toast;

import com.jjoe64.graphview.GraphView;
import com.jjoe64.graphview.helper.DateAsXAxisLabelFormatter;
import com.jjoe64.graphview.helper.StaticLabelsFormatter;
import com.jjoe64.graphview.series.DataPoint;
import com.jjoe64.graphview.series.LineGraphSeries;

import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Date;
import java.util.List;

public class Graph extends AppCompatActivity {
    LineGraphSeries<DataPoint> series,min,max;
    GraphView graph;
    String[] data;
    ArrayList spinnerlist;
    SeekBar seekBar;
    Spinner spinner;
   // StaticLabelsFormatter staticLabelsFormatter;
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
        setContentView(R.layout.activity_graph);
        graph = findViewById(R.id.graph);
        /*series= new LineGraphSeries<>();
               // new DataPoint(d3, 3)
        graph.addSeries(series);*/

        /*series= new LineGraphSeries<>(new DataPoint[] {
                new DataPoint(0, 1),
                new DataPoint(1, 5),
                new DataPoint(2, 3)
        });
        //
        graph.addSeries(series);*/
        graph.getGridLabelRenderer().setLabelFormatter(new DateAsXAxisLabelFormatter(this));
       // graph.getGridLabelRenderer().setNumHorizontalLabels(3);
        graph.getGridLabelRenderer().setHumanRounding(false);
        graph.getViewport().setScalable(true);
        graph.getViewport().setScalableY(true);
        data= MainActivity.client.Send("get arduino data").split(";");
        try {
            Thread.sleep(500);
        } catch (InterruptedException e) {
            Toast.makeText(this, "Nope", Toast.LENGTH_SHORT).show();
        }
        if(data[0].equals("Error"))return;
        Spinnerinitalisieren();
        seekBar= findViewById(R.id.seekBar);
        seekBar.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {
            int position;
            @Override
            public void onProgressChanged(SeekBar seekBar, int i, boolean b) {
                if(position!=seekBar.getProgress()){
                    DeleteOldGraph();
                }
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {
                position=seekBar.getProgress();
            }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {
                if(position!=seekBar.getProgress()){
                    DeleteOldGraph();
                    SetGraph(seekBar.getProgress(),Integer.parseInt(spinnerlist.get(spinner.getSelectedItemPosition()).toString().split("-")[0]));
                }
            }
        });


    }

    private void Spinnerinitalisieren() {
        spinner = findViewById(R.id.spinnergraph);
        List<String> help = new ArrayList<>();
        spinnerlist = new ArrayList<>();
        for (String aData : data) {
            boolean is = true;
            for (int g = 0; g < help.size() && is; g++) {
                if (aData.split("_")[0].equals(help.get(g))) {
                    is = false;
                }
            }
            if (is) {
                help.add(aData.split("_")[0]);
                spinnerlist.add(aData.split("_")[0] + "-" + aData.split("_")[1]);
            }
        }
        ArrayAdapter<String> arrayAdapter = new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, spinnerlist);
        arrayAdapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(arrayAdapter);
        spinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> adapterView, View view, int i, long l) {
                DeleteOldGraph();
                SetGraph(seekBar.getProgress(),Integer.parseInt(spinnerlist.get(i).toString().split("-")[0]));
            }

            @Override
            public void onNothingSelected(AdapterView<?> adapterView) {

            }

        });
    }

    private  void DeleteOldGraph(){
        graph.getGridLabelRenderer().resetStyles();
        graph.removeSeries(series);
        graph.removeSeries(min);
        graph.removeSeries(max);
    }
    private void SetGraph(int mode, int arduinoId){
        if(data.length<2){
            return;
        }
        series=new LineGraphSeries<>();
        min=new LineGraphSeries<>();
        max=new LineGraphSeries<>();
        int counter=0;
        for (String aData : data) {
            if (aData.split("_")[0].equals(arduinoId + "")) {
                SimpleDateFormat format = new SimpleDateFormat("yyyy.MM.dd HH:mm:ss");
                try {
                    Log.e("graph",aData.split("_")[2]);
                    if(mode!=3) {
                        min.appendData(new DataPoint(format.parse(aData.split("_")[2]), Float.parseFloat(aData.split("_")[mode + 4].replace(",", "."))), false, ++counter);
                    }
                    max.appendData(new DataPoint(format.parse(aData.split("_")[2]) , Float.parseFloat(aData.split("_")[mode + 7].replace(",","."))), false, ++counter);
                    series.appendData(new DataPoint(format.parse(aData.split("_")[2]) , Float.parseFloat(aData.split("_")[mode + 3].replace(",","."))), false, ++counter);
                } catch (ParseException e) {
                    Log.e("graph","no");
                    e.printStackTrace();
                }
            }
        }
        //staticLabelsFormatter = new StaticLabelsFormatter(graph);
        //staticLabelsFormatter.setHorizontalLabels(new String[] {"test", "01.01.2001", "new"});
        //graph.getGridLabelRenderer().setLabelFormatter(staticLabelsFormatter);
        Paint paint = new Paint();
        paint.setStyle(Paint.Style.STROKE);
        paint.setColor(Color.RED);
        min.setCustomPaint(paint);
        max.setCustomPaint(paint);
        min.setThickness(8);
        max.setThickness(8);
        switch(mode){
            case 0: series.setColor(Color.RED); graph.addSeries(min);break;
            case 1: series.setColor(Color.BLUE);graph.addSeries(min);break;
            case 2: series.setColor(Color.BLACK); graph.addSeries(min);break;
            case 3: series.setColor(Color.YELLOW);break;
        }
        graph.addSeries(max);
        series.setDrawDataPoints(true);
        series.setDataPointsRadius(10);
        series.setThickness(8);
        graph.addSeries(series);
    }

    public void back_Click(View view) {
        Intent intent = new Intent(this, Main3Activity.class);
        startActivity(intent);
    }
}
