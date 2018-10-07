package com.example.win7.gartenhausapp;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.EditText;
import android.widget.Toast;

public class Edit_Plant extends AppCompatActivity {

    private Client client;
    private int ID;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_edit__plant);
        client=MainActivity.client;
        ID=getIntent().getIntExtra("ID",-1); //mitgesendete ID auslesen
        InitClickListner();
        if(ID==-1) return; //Wenn neu dann return
        findViewById(R.id.imVDeletePlaint).setVisibility(View.VISIBLE);
        try {
            Thread.sleep(500);
        } catch (InterruptedException e) {
            Toast.makeText(this,"Nope",Toast.LENGTH_SHORT).show();
        }
        String[] help=client.Send("get all_"+ID).split("_"); //Daten bekommen
        try {
            Thread.sleep(500);
        } catch (InterruptedException e) {
            Toast.makeText(this,"Nope",Toast.LENGTH_SHORT).show();
        }
        Log.e("help",help.length+"");
        if(help.length<2)return;
        //Text setzen in die zugehörigen TextViews
        ((EditText)findViewById(R.id.edTName)).setText(help[0]);
        ((EditText)findViewById(R.id.edTminTemp)).setText(help[1]);
        ((EditText)findViewById(R.id.edTmaxTemp)).setText(help[2]);
        ((EditText)findViewById(R.id.edTminFeucht)).setText(help[3]);
        ((EditText)findViewById(R.id.edTmaxFeucht)).setText(help[4]);
        ((EditText)findViewById(R.id.edTminHumid)).setText(help[5]);
        ((EditText)findViewById(R.id.edTmaxHumid)).setText(help[6]);
        ((EditText)findViewById(R.id.edTminUV)).setText(help[7]);
        ((EditText)findViewById(R.id.edTmaxUV)).setText(help[8]);
    }
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.menu,menu); //Menü initalisieren
        return super.onCreateOptionsMenu(menu);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        startActivity(new Intent(getApplicationContext(),MainActivity.class)); //Menü Click Listener
        return super.onOptionsItemSelected(item);
    }
    private void InitClickListner(){
        findViewById(R.id.imVSavePlaint).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) { //Auslesen der TextViews
                String name=((EditText)findViewById(R.id.edTName)).getText().toString();
                String MinTemp=((EditText)findViewById(R.id.edTminTemp)).getText().toString();
                String MaxTemp=((EditText)findViewById(R.id.edTmaxTemp)).getText().toString();
                String MinFeucht=((EditText)findViewById(R.id.edTminFeucht)).getText().toString();
                String MaxFeucht=((EditText)findViewById(R.id.edTmaxFeucht)).getText().toString();
                String MinHumid=((EditText)findViewById(R.id.edTminHumid)).getText().toString();
                String MaxHumid=((EditText)findViewById(R.id.edTmaxHumid)).getText().toString();
                String MaxUV=((EditText)findViewById(R.id.edTmaxUV)).getText().toString();
                String MinUV=((EditText)findViewById(R.id.edTminUV)).getText().toString();

                //Wenn etwas nicht ausgefüllt, FEhlermeldung an Benutzer
                if(name.trim().equals("") ||MinTemp.equals("")||MaxTemp.equals("")||MinFeucht.equals("")||
                        MaxFeucht.equals("")||MinHumid.equals("")||MaxHumid.equals("")||MaxUV.equals("")||MinUV.equals("")){
                   Toast.makeText(getApplicationContext(),getString(R.string.edit_all),Toast.LENGTH_LONG).show();
                    return;
                }
                if(ID!=-1) //wenn bearbeiten dann einzeln setzen
                {
                    Log.e("Change",client.Send("set name_"+ID+"_"+name));
                    try {
                        Thread.sleep(500);
                    } catch (InterruptedException e) {
                        Toast.makeText(getApplicationContext(),"Nope",Toast.LENGTH_SHORT).show();
                    }
                    Log.e("Change",client.Send("set mintemp_"+ID+"_"+MinTemp));
                    try {
                        Thread.sleep(500);
                    } catch (InterruptedException e) {
                        Toast.makeText(getApplicationContext(),"Nope",Toast.LENGTH_SHORT).show();
                    }
                    Log.e("Change",client.Send("set maxtemp_"+ID+"_"+MaxTemp));
                    try {
                        Thread.sleep(500);
                    } catch (InterruptedException e) {
                        Toast.makeText(getApplicationContext(),"Nope",Toast.LENGTH_SHORT).show();
                    }
                    Log.e("Change",client.Send("set minfeucht_"+ID+"_"+MinFeucht));
                    try {
                        Thread.sleep(500);
                    } catch (InterruptedException e) {
                        Toast.makeText(getApplicationContext(),"Nope",Toast.LENGTH_SHORT).show();
                    }
                    Log.e("Change",client.Send("set maxfeucht_"+ID+"_"+MaxFeucht));
                    try {
                        Thread.sleep(500);
                    } catch (InterruptedException e) {
                        Toast.makeText(getApplicationContext(),"Nope",Toast.LENGTH_SHORT).show();
                    }
                    Log.e("Change",client.Send("set minhumid_"+ID+"_"+MinHumid));
                    try {
                        Thread.sleep(500);
                    } catch (InterruptedException e) {
                        Toast.makeText(getApplicationContext(),"Nope",Toast.LENGTH_SHORT).show();
                    }
                    Log.e("Change",client.Send("set maxhumid_"+ID+"_"+MaxHumid));
                    try {
                        Thread.sleep(500);
                    } catch (InterruptedException e) {
                        Toast.makeText(getApplicationContext(),"Nope",Toast.LENGTH_SHORT).show();
                    }
                    Log.e("Change",client.Send("set minuv_"+ID+"_"+MinUV));
                    try {
                        Thread.sleep(500);
                    } catch (InterruptedException e) {
                        Toast.makeText(getApplicationContext(),"Nope",Toast.LENGTH_SHORT).show();
                    }
                    Log.e("Change",client.Send("set maxuv_"+ID+"_"+MaxUV));

                }
                    else
                        //neue Pflanze erstellen
                Log.e("Add",client.Send("new_"+name+"_"+MinTemp+"_"+MaxTemp+"_"+MinFeucht+"_"+MaxFeucht+"_"+MinHumid+"_"+MaxHumid+"_"+MinUV+"_"+MaxUV));
            Close();
            }
        });
        findViewById(R.id.imVClosePlaint).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
            Close();
            }
        });//Schließen
        if(ID!=-1)
        findViewById(R.id.imVDeletePlaint).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Log.e("Delete",client.Send("delete_"+ID));
                Close();
            }
        });//Löschen
    }

    private void Close() {//Schließen
        client.Stop();//stoppen vom client
        Intent intent = new Intent(this,Main2Activity.class); //Main2Activity starten
        startActivity(intent);
    }

    public void btnClick_Tastatur(View view) { //Tastatur schließen, wenn auf Display gedrückt
        InputMethodManager imm = (InputMethodManager)getSystemService(Context.INPUT_METHOD_SERVICE);
        try{
            if (imm != null) {
                imm.hideSoftInputFromWindow(getCurrentFocus().getWindowToken(), 0);
            }
        }catch(NullPointerException e){e.printStackTrace();}

    }
}
