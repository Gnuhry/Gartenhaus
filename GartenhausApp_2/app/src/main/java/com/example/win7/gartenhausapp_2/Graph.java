package com.example.win7.gartenhausapp_2;

import android.content.Intent;
import android.graphics.Color;
import android.graphics.Paint;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
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
import com.jjoe64.graphview.series.DataPoint;
import com.jjoe64.graphview.series.DataPointInterface;
import com.jjoe64.graphview.series.LineGraphSeries;
import com.jjoe64.graphview.series.OnDataPointTapListener;
import com.jjoe64.graphview.series.Series;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.Locale;

public class Graph extends AppCompatActivity {
    LineGraphSeries<DataPoint> series, min, max;
    GraphView graph;
    String[] data;
    ArrayList<String> spinnerlist = new ArrayList<>();
    SeekBar seekBar;
    Spinner spinner;
    Graph grap;

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
        setContentView(R.layout.activity_graph);
        graph = findViewById(R.id.graph);
        graph.getGridLabelRenderer().setLabelFormatter(new DateAsXAxisLabelFormatter(this));
        graph.getGridLabelRenderer().setHumanRounding(false);
        graph.getViewport().setScalable(true);
        graph.getViewport().setScalableY(true);
        data = MainActivity.client.Send("get arduino data").split(";");
        if (data[0].equals(getString(R.string.error))) {
            graph.setEnabled(false);
            findViewById(R.id.seekBar).setEnabled(false);
            ArrayList<String> strings = new ArrayList<>();
            strings.add(getString(R.string.error));
            ArrayAdapter<String> arrayAdapter = new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, strings);
            arrayAdapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
            ((Spinner) findViewById(R.id.spinnergraph)).setAdapter(arrayAdapter);
            findViewById(R.id.spinnergraph).setEnabled(false);
            return;
        }
        Spinnerinitalisieren();
        seekBar = findViewById(R.id.seekBar);
        seekBar.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {
            int position;

            @Override
            public void onProgressChanged(SeekBar seekBar, int i, boolean b) {
                if (position != seekBar.getProgress()) {
                    DeleteOldGraph();
                }
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {
                position = seekBar.getProgress();
            }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {
                if (position != seekBar.getProgress()) {
                    DeleteOldGraph();
                    SetGraph(seekBar.getProgress(), spinner.getSelectedItemPosition());
                }
            }
        });


    }

    private void Spinnerinitalisieren() {
        spinner = findViewById(R.id.spinnergraph);
        for (String aData : data) {
            if (!(aData.toCharArray()[0] == ':')) {
                break;
            } else {
                spinnerlist.add(aData.split("_")[0].substring(1));
            }
        }
        ArrayAdapter<String> arrayAdapter = new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, spinnerlist);
        arrayAdapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(arrayAdapter);
        spinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> adapterView, View view, int i, long l) {
                DeleteOldGraph();
                SetGraph(seekBar.getProgress(), i);
            }

            @Override
            public void onNothingSelected(AdapterView<?> adapterView) {

            }

        });
    }

    private void DeleteOldGraph() {
        graph.getGridLabelRenderer().resetStyles();
        graph.removeSeries(series);
        graph.removeSeries(min);
        graph.removeSeries(max);
    }

    private void SetGraph(int mode, int splitter) {
        int counter = 0;
        series = new LineGraphSeries<>();
        min = new LineGraphSeries<>();
        max = new LineGraphSeries<>();
        for (String aData : data) {
            if (!(aData.toCharArray()[0] == ':')) {
                counter++;
                SimpleDateFormat format = new SimpleDateFormat("HH:mm:ss", Locale.GERMAN);
                try {
                    Date date = format.parse(aData.split("_")[2].substring(11));
                    Log.e("Graph", date.toString());
                    Calendar calendar = Calendar.getInstance();
                    calendar.setTime(date);
                    calendar.add(Calendar.DAY_OF_YEAR, Integer.parseInt(aData.split("_")[2].substring(0, 2)) - 1);//split(".")[0]));
                    calendar.add(Calendar.MONTH, Integer.parseInt(aData.split("_")[2].substring(3, 5)) - 1);
                    calendar.add(Calendar.YEAR, Integer.parseInt(aData.split("_")[2].substring(6, 10)) - 1970);
                    date = calendar.getTime();
                    if (data[splitter].split("-")[0].substring(1).equals(aData.split("_")[0])) {
                        if (data[splitter].split("_").length > 1) {
                            max.appendData(new DataPoint(date, Float.parseFloat(data[splitter].split("_")[mode + 4].replace(",", "."))), false, counter);
                            if (mode != 3) {
                                min.appendData(new DataPoint(date, Float.parseFloat(data[splitter].split("_")[mode + 1].replace(",", "."))), false, counter);
                            }
                        }
                        if (Float.parseFloat(aData.split("_")[mode + 3].replace(",", ".")) != -100) {
                            series.appendData(new DataPoint(date, Float.parseFloat(aData.split("_")[mode + 3].replace(",", "."))), false, counter);
                        }
                    }
                } catch (ParseException e) {
                    e.printStackTrace();
                }
            }
        }
        if (data[splitter].split("_").length > 1) {
            Paint paint = new Paint();
            paint.setStyle(Paint.Style.FILL_AND_STROKE);
            paint.setStrokeWidth(2);
            paint.setColor(Color.RED);
            min.setCustomPaint(paint);
            max.setCustomPaint(paint);
            min.setThickness(8);
            max.setThickness(8);
            switch (mode) {
                case 0:
                    series.setColor(Color.RED);
                    graph.addSeries(min);
                    break;
                case 1:
                    series.setColor(Color.BLUE);
                    graph.addSeries(min);
                    break;
                case 2:
                    series.setColor(Color.BLACK);
                    graph.addSeries(min);
                    break;
                case 3:
                    series.setColor(Color.YELLOW);
                    break;
            }
            graph.addSeries(max);
        }
        series.setDrawDataPoints(true);
        series.setDataPointsRadius(10);
        series.setThickness(8);
        grap = this;
        series.setOnDataPointTapListener(new OnDataPointTapListener() {
            @Override
            public void onTap(Series series, DataPointInterface dataPoint) {
                Toast.makeText(grap, new SimpleDateFormat("kk:mm.ss dd.MM.yyyy", Locale.GERMAN).format(new Date((long) dataPoint.getX())) + "/" + String.format("%.2f", dataPoint.getY()), Toast.LENGTH_SHORT).show();
            }
        });
        graph.addSeries(series);
    }

    public void Reload(View view) {
        startActivity(getIntent());
    }

}
