package com.example.win7.gartenhausapp_2;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;

public class Credits extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_credits);
        SetDownMenu();
    }
    private void SetDownMenu(){
        findViewById(R.id.downbar).setBackgroundResource(R.drawable.home);
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
}
