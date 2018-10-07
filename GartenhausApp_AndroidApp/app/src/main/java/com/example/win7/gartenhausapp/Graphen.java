package com.example.win7.gartenhausapp;

import android.graphics.DashPathEffect;
import android.graphics.Paint;
import android.os.Bundle;
import android.support.v4.content.res.ResourcesCompat;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.CheckBox;
import android.widget.Spinner;
import android.widget.Toast;

import com.jjoe64.graphview.DefaultLabelFormatter;
import com.jjoe64.graphview.GraphView;
import com.jjoe64.graphview.series.DataPoint;
import com.jjoe64.graphview.series.DataPointInterface;
import com.jjoe64.graphview.series.LineGraphSeries;
import com.jjoe64.graphview.series.OnDataPointTapListener;
import com.jjoe64.graphview.series.Series;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Date;
import java.util.List;
import java.util.Locale;


public class Graphen extends AppCompatActivity {

    private SimpleDateFormat simpleDateFormat=new SimpleDateFormat("dd.MM hh:mm", Locale.GERMAN),
            simpleDateFormat2=new SimpleDateFormat("mm:hh - EEEEEEE dd.MMMM yyyy", Locale.GERMAN);

    private GraphView graphView;
    private LineGraphSeries<DataPoint> seriesTemp,seriesFeucht,seriesHumid,seriesUV, seriesTemp2,seriesFeucht2,seriesHumid2,seriesUV2;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_graphen);
        GraphenFullen();
    }

    private void GraphenFullen() {
        graphView= findViewById(R.id.graph);
        graphView.getViewport().setScalable(true);
        graphView.getViewport().setScalableY(true);
        graphView.getGridLabelRenderer().setNumHorizontalLabels(2);
        graphView.getGridLabelRenderer().setHumanRounding(false);
        graphView.getViewport().computeScroll();
        graphView.getGridLabelRenderer().setLabelFormatter(new DefaultLabelFormatter(){
            @Override
            public String formatLabel(double value, boolean isValueX) {
                if(isValueX) return simpleDateFormat.format(new Date((long) value));
                return super.formatLabel(value, false);
            }
        });
        graphView.getGridLabelRenderer().setLabelsSpace(10);
        graphView.getGridLabelRenderer().setLabelHorizontalHeight(10);


        seriesTemp=new LineGraphSeries<>();
        seriesTemp.setTitle("Temperatur");
        seriesTemp.setColor(ResourcesCompat.getColor(getResources(), R.color.Temperatur, null));
        seriesTemp.setDrawDataPoints(true);
        seriesTemp.setDataPointsRadius(10);
        seriesTemp.setThickness(8);
        seriesTemp.setOnDataPointTapListener(new OnDataPointTapListener() {
            @Override
            public void onTap(Series seriesTemp, DataPointInterface dataPoint) {
                Date date=new Date();
                date.setTime((long) dataPoint.getX());
                Toast.makeText(getApplicationContext(), simpleDateFormat2.format(date)+", "+dataPoint.getY()+"°C", Toast.LENGTH_SHORT).show();
            }
        });
        
        seriesFeucht=new LineGraphSeries<>();
        seriesFeucht.setTitle("Feuchtigkeit");
        seriesFeucht.setColor(ResourcesCompat.getColor(getResources(), R.color.Feucht, null));
        seriesFeucht.setDrawDataPoints(true);
        seriesFeucht.setDataPointsRadius(10);
        seriesFeucht.setThickness(8);
        seriesFeucht.setOnDataPointTapListener(new OnDataPointTapListener() {
            @Override
            public void onTap(Series seriesFeucht, DataPointInterface dataPoint) {
                Date date=new Date();
                date.setTime((long) dataPoint.getX());
                Toast.makeText(getApplicationContext(), simpleDateFormat2.format(date)+", "+dataPoint.getY()+"g/m³", Toast.LENGTH_SHORT).show();
            }
        });
        
        seriesHumid=new LineGraphSeries<>();
        seriesHumid.setTitle("Humid");
        seriesHumid.setColor(ResourcesCompat.getColor(getResources(), R.color.Humid, null));
        seriesHumid.setDrawDataPoints(true);
        seriesHumid.setDataPointsRadius(10);
        seriesHumid.setThickness(8);
        seriesHumid.setOnDataPointTapListener(new OnDataPointTapListener() {
            @Override
            public void onTap(Series seriesHumid, DataPointInterface dataPoint) {
                Date date=new Date();
                date.setTime((long) dataPoint.getX());
                Toast.makeText(getApplicationContext(), simpleDateFormat2.format(date)+", "+dataPoint.getY()+"g/m³", Toast.LENGTH_SHORT).show();
            }
        });
        
        seriesUV=new LineGraphSeries<>();
        seriesUV.setTitle("UV");
        seriesUV.setColor(ResourcesCompat.getColor(getResources(), R.color.UV, null));
        seriesUV.setDrawDataPoints(true);
        seriesUV.setDataPointsRadius(10);
        seriesUV.setThickness(8);
        seriesUV.setOnDataPointTapListener(new OnDataPointTapListener() {
            @Override
            public void onTap(Series seriesUV, DataPointInterface dataPoint) {
                Date date=new Date();
                date.setTime((long) dataPoint.getX());
                Toast.makeText(getApplicationContext(), simpleDateFormat2.format(date)+", "+dataPoint.getY()+"UV", Toast.LENGTH_SHORT).show();
            }
        });
        Paint paint = new Paint();
        paint.setStyle(Paint.Style.STROKE);
        //paint.setStrokeWidth(10);
        paint.setPathEffect(new DashPathEffect(new float[]{8, 5}, 0));
       
        seriesTemp2=new LineGraphSeries<>();
        seriesTemp2.setTitle("Temperatur2");
        seriesTemp2.setColor(ResourcesCompat.getColor(getResources(), R.color.Temperatur, null));
        seriesTemp2.setDrawDataPoints(true);
        seriesTemp2.setDataPointsRadius(10);
        seriesTemp2.setThickness(8);
        seriesTemp2.setOnDataPointTapListener(new OnDataPointTapListener() {
            @Override
            public void onTap(Series seriesTemp, DataPointInterface dataPoint) {
                Date date=new Date();
                date.setTime((long) dataPoint.getX());
                Toast.makeText(getApplicationContext(), simpleDateFormat2.format(date)+", "+dataPoint.getY()+"°C", Toast.LENGTH_SHORT).show();
            }
        });

        seriesFeucht2=new LineGraphSeries<>();
        seriesFeucht2.setTitle("Feuchtigkeit2");
        seriesFeucht2.setColor(ResourcesCompat.getColor(getResources(), R.color.Feucht, null));
        seriesFeucht2.setDrawDataPoints(true);
        seriesFeucht2.setDataPointsRadius(10);
        seriesFeucht2.setThickness(8);
        seriesFeucht2.setOnDataPointTapListener(new OnDataPointTapListener() {
            @Override
            public void onTap(Series seriesFeucht, DataPointInterface dataPoint) {
                Date date=new Date();
                date.setTime((long) dataPoint.getX());
                Toast.makeText(getApplicationContext(), simpleDateFormat2.format(date)+", "+dataPoint.getY()+"g/m³", Toast.LENGTH_SHORT).show();
            }
        });

        seriesHumid2=new LineGraphSeries<>();
        seriesHumid2.setTitle("Humid2");
        seriesHumid2.setColor(ResourcesCompat.getColor(getResources(), R.color.Humid, null));
        seriesHumid2.setDrawDataPoints(true);
        seriesHumid2.setDataPointsRadius(10);
        seriesHumid2.setThickness(8);
        seriesHumid2.setOnDataPointTapListener(new OnDataPointTapListener() {
            @Override
            public void onTap(Series seriesHumid, DataPointInterface dataPoint) {
                Date date=new Date();
                date.setTime((long) dataPoint.getX());
                Toast.makeText(getApplicationContext(), simpleDateFormat2.format(date)+", "+dataPoint.getY()+"g/m³", Toast.LENGTH_SHORT).show();
            }
        });

        seriesUV2=new LineGraphSeries<>();
        seriesUV2.setTitle("UV2");
        seriesUV2.setColor(ResourcesCompat.getColor(getResources(), R.color.UV, null));
        seriesUV2.setDrawDataPoints(true);
        seriesUV2.setDataPointsRadius(10);
        seriesUV2.setThickness(8);
        seriesUV2.setOnDataPointTapListener(new OnDataPointTapListener() {
            @Override
            public void onTap(Series seriesUV, DataPointInterface dataPoint) {
                Date date=new Date();
                date.setTime((long) dataPoint.getX());
                Toast.makeText(getApplicationContext(), simpleDateFormat2.format(date)+", "+dataPoint.getY()+"UV", Toast.LENGTH_SHORT).show();
            }
        });
        seriesTemp2.setCustomPaint(paint);
        seriesHumid2.setCustomPaint(paint);
        seriesFeucht2.setCustomPaint(paint);
        seriesUV2.setCustomPaint(paint);
//       seriesTemp2.appendData(new DataPoint(new Date(1,1,1,3,2,0),2),false,10);
//        seriesTemp2.appendData(new DataPoint(new Date(1,3,2,3,3,0),3),false,10);
//        seriesTemp.appendData(new DataPoint(new Date(1,1,1,3,2,0),100),false,10);
//        seriesTemp.appendData(new DataPoint(new Date(1,3,2,3,3,0),1),false,10);
        Laden(1,false);
        graphView.addSeries(seriesTemp);
        graphView.addSeries(seriesFeucht);
        graphView.addSeries(seriesHumid);
        graphView.addSeries(seriesUV);
        graphView.addSeries(seriesTemp2);
        graphView.addSeries(seriesFeucht2);
        graphView.addSeries(seriesHumid2);
        graphView.addSeries(seriesUV2);



        Init();
//        seriesTemp2.appendData(new DataPoint(new Date(2,2,2),2),false,1000);
//        seriesTemp2.appendData(new DataPoint(new Date(2,2,3),3),false,1000);
//        seriesTemp.appendData(new DataPoint(new Date(0,1,1),1),false,1000);
//        seriesTemp.appendData(new DataPoint(new Date(1,1,1),1),false,1000);
    }

    private void Init() {
        String [] ID =MainActivity.client.Send("get arduinoids").split("_");
        if(!ID[0].equals("No Inet")){
        List<String> IDS= new ArrayList<>(Arrays.asList(ID));
        ArrayAdapter<String> arrayAdapter= new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, IDS);
        arrayAdapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        ((Spinner)findViewById(R.id.spinnerGraph)).setAdapter(arrayAdapter);
      //  ((Spinner)findViewById(R.id.spinnerGraph2)).setAdapter(arrayAdapter);
           // ((Spinner)findViewById(R.id.spinnerGraph2)).setVisibility(View.GONE);
        ((Spinner)findViewById(R.id.spinnerGraph)).setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> adapterView, View view, int i, long l) {
                try{
                Laden(Integer.parseInt((String)adapterView.getItemAtPosition(i)),false);}catch(Exception ignored){}

            }

            @Override
            public void onNothingSelected(AdapterView<?> adapterView) {

            }

        });
       /* ((Spinner)findViewById(R.id.spinnerGraph2)).setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> adapterView, View view, int i, long l) {
  try{
                Laden(Integer.parseInt((String)adapterView.getItemAtPosition(i)),true);}catch(Exception e){return;}

            }

            @Override
            public void onNothingSelected(AdapterView<?> adapterView) {

            }

        });*/
    }}

    private void Laden(int indexarduino,boolean Gestrichelt) {
        //TODO Ausprobieren
        SimpleDateFormat dateFormat=new SimpleDateFormat("hh:mm dd.MM.yyyy", Locale.GERMAN);
        Client client=MainActivity.client;
        int lenght=Integer.parseInt(client.Send("get length_"+indexarduino));
        try {
            Thread.sleep(500); //warten auf Antwort
        } catch (InterruptedException e) {
            Toast.makeText(this,"Nope",Toast.LENGTH_SHORT).show();
        }
        String data[]=new String[lenght];
        Date date=new Date();
        for(int f=0;f<lenght-1;f++){
            data[f]=client.Send("get data_"+indexarduino+"_"+(f+1));
            try {
                Thread.sleep(500); //warten auf Antwort
            } catch (InterruptedException e) {
                Toast.makeText(this,"Nope",Toast.LENGTH_SHORT).show();
            }
        }
        for(String data_:data){
            String dat=data_.split("#")[1];
            try {
                date=dateFormat.parse(data_.split("#")[0]);
            } catch (ParseException e) {
                e.printStackTrace();
            }
            if(!Gestrichelt) {
                Log.e("Malen","Malen------------");
                seriesTemp.appendData(new DataPoint(date, Float.parseFloat(dat.split("-")[0])), false, 1000);
                seriesFeucht.appendData(new DataPoint(date, Float.parseFloat(dat.split("-")[1])), false, 1000);
                seriesHumid.appendData(new DataPoint(date, Float.parseFloat(dat.split("-")[2])), false, 1000);
                seriesUV.appendData(new DataPoint(date, Float.parseFloat(dat.split("-")[3])), false, 1000);
            }
            else{
                seriesTemp2.appendData(new DataPoint(date, Float.parseFloat(dat.split("-")[0])), false, 1000);
                seriesFeucht2.appendData(new DataPoint(date, Float.parseFloat(dat.split("-")[1])), false, 1000);
                seriesHumid2.appendData(new DataPoint(date, Float.parseFloat(dat.split("-")[2])), false, 1000);
                seriesUV2.appendData(new DataPoint(date, Float.parseFloat(dat.split("-")[3])), false, 1000);
            }
        }
    }

    public void btnClick_RTemp(View view) {
        if(((CheckBox)view).isChecked()) graphView.addSeries(seriesTemp);
       // else  graphView.removeSeries(seriesTemp);
    }

    public void btnClick_RFeucht(View view) {
        if(((CheckBox)view).isChecked()) graphView.addSeries(seriesFeucht);
       // else  graphView.removeSeries(seriesFeucht);
    }

    public void btnClick_RHumid(View view) {
        if(((CheckBox)view).isChecked()) graphView.addSeries(seriesHumid);
      //  else  graphView.removeSeries(seriesHumid);
    }

    public void btnClick_RUV(View view) {
        if(((CheckBox)view).isChecked()) graphView.addSeries(seriesUV);
      //  else  graphView.removeSeries(seriesUV);
    }
}
