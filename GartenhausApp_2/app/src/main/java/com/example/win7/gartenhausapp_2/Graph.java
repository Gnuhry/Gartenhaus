package com.example.win7.gartenhausapp_2;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.widget.Toast;

import com.jjoe64.graphview.GraphView;
import com.jjoe64.graphview.helper.DateAsXAxisLabelFormatter;
import com.jjoe64.graphview.series.DataPoint;
import com.jjoe64.graphview.series.LineGraphSeries;

public class Graph extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_graph);
        GraphView graph = findViewById(R.id.graph);
        LineGraphSeries<DataPoint> series = new LineGraphSeries<>();
               // new DataPoint(d3, 3)
        graph.addSeries(series);

        graph.getGridLabelRenderer().setLabelFormatter(new DateAsXAxisLabelFormatter(this));
        graph.getGridLabelRenderer().setNumHorizontalLabels(3);

        graph.getGridLabelRenderer().setHumanRounding(false);

        String[] data = MainActivity.client.Send("get arduino data").split(";");
        try {
            Thread.sleep(500);
        } catch (InterruptedException e) {
            Toast.makeText(this, "Nope", Toast.LENGTH_SHORT).show();
        }

    }
}
