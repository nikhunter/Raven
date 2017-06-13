package com.raven_gps.raven;

import android.app.Activity;
import android.app.AlertDialog;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.content.DialogInterface;
import android.content.Intent;
import android.graphics.Path;
import android.os.Bundle;
import android.os.Debug;
import android.os.Environment;
import android.os.Handler;
import android.support.annotation.RequiresPermission;
import android.util.JsonReader;
import android.util.Log;
import android.view.View;
import android.widget.CompoundButton;
import android.widget.GridLayout;
import android.widget.TextView;
import android.widget.EditText;
import android.widget.Button;
import android.widget.Toast;
import android.widget.ToggleButton;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.sql.DriverManager;
import java.sql.SQLException;

import android.app.Activity;
import android.content.Context;
import android.os.AsyncTask;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import org.w3c.dom.Text;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.Reader;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Set;
import java.util.UUID;

public class MainActivity extends Activity {
	TextView date;
	TextView time;
	TextView kmh;
	TextView rpm;
	TextView lat;
	TextView lng;
	TextView arrayCount;
	ToggleButton loggingToggleBtn;
	TextView log;
	GridLayout gridLayout;
	TextView myLabel;
	EditText myTextbox;

	BluetoothAdapter mBluetoothAdapter;
	BluetoothSocket mmSocket;
	BluetoothDevice mmDevice;
	OutputStream mmOutputStream;
	InputStream mmInputStream;
	Thread workerThread;
	byte[] readBuffer;
	TextView Log;
	int readBufferPosition;
	int counter;

	JSONArray jsonArray;

	volatile boolean stopWorker;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);
		gridLayout = (GridLayout) findViewById(R.id.gridLayout);

		date = (TextView) findViewById(R.id.date);
		time = (TextView) findViewById(R.id.time);
		kmh = (TextView) findViewById(R.id.kmh);
		rpm = (TextView) findViewById(R.id.rpm);
		lat = (TextView) findViewById(R.id.lat);
		lng = (TextView) findViewById(R.id.lng);
		loggingToggleBtn = (ToggleButton) findViewById(R.id.LoggingToggleBtn);
		arrayCount = (TextView) findViewById(R.id.arrayCount);
		log = (TextView) findViewById(R.id.log);


       /* // NOT RECOMMENDED NOT FOR PROD USE ALARM ALARM
		static final String url = "jdbc:mysql://raven-gps.com:3306/raven";
        static final String user = "root";
        static final String pass = "Raven123";
        // NOT RECOMMENDED NOT FOR PROD USE ALARM ALARM*/

		loggingToggleBtn.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
			@Override
			public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
				switch (String.valueOf(isChecked).toUpperCase()) {
					case "TRUE":        // ENABLED

						jsonArray = new JSONArray();

						break;
					case "FALSE":       // DISABLED
						AlertDialog.Builder adb = new AlertDialog.Builder(MainActivity.this);
						adb.setTitle("Save File");

                      /*  CharSequence[] options = {
                                "Upload To SQL",
                                "Do nothing",
                                "Blah",
                                "Blahh"
                        };

                        final boolean[] optionsBool = new boolean[options.length];
                        adb.setSingleChoiceItems(options, 0, new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialog, int which) {
                                for (int i = 0; i<optionsBool.length;i++){
                                    if (i == which){
                                        optionsBool[i] = true;
                                    }else{
                                        optionsBool[i] = false;
                                    }
                                }
                            }
                        });*/

						adb.setPositiveButton("Upload To SQL", new DialogInterface.OnClickListener() {
							@Override
							public void onClick(DialogInterface dialog, int which) {
								log.setText(jsonArray.toString());

								DateFormat dateFormat = new SimpleDateFormat("yyyy/MM/dd-HH-mm-ss");
								Date date = new Date();
								writeToFile(jsonArray.toString(), dateFormat.format(date) + ".json");

							}
						});

						adb.setNegativeButton("Discard", null);
						adb.setCancelable(false);
						adb.show();
						break;
				}
			}
		});


	}



	public void writeToFile(String data, String name) {
		// Get the directory for the user's public pictures directory.
		File sdcard = Environment.getExternalStorageDirectory();
		File path = new File(sdcard.getAbsolutePath() + "/RavenLogs");

		// Make sure the path directory exists.
		try {
			/*
			if (!path.exists()) {
				// Make it, if it doesn't exit
				path.mkdir();
				WriteLog("Path not exists");
			}*/

			if (!path.exists()){
				WriteLog(path.getAbsolutePath() + " Doesn't exists");
				if(path.mkdirs()){
					WriteLog("path created");
				}else{
					WriteLog("Path could not be created");
				}
			}

			File file = new File(path, name);
			WriteLog(file.getAbsolutePath());
			FileWriter writer = new FileWriter(file);
			writer.append(data);
			writer.flush();
			writer.close();
		} catch (Exception e) {
			WriteLog(e.getMessage());
		}
		// Save your stream, don't forget to flush() it before closing it.
	}

	public void CloseBTBtn_OnClick(View v) {
		try {
			closeBT();
		} catch (IOException ex) {
		}
	}

	public void OpenBTBtn_OnClick(View v) {
		try {
			findBT();
			openBT();
		} catch (IOException ex) {
			AlertDialog.Builder dlgAlert = new AlertDialog.Builder(MainActivity.super.getParent());
			dlgAlert.setMessage(ex.getMessage());
			dlgAlert.setTitle("Error Connecting to BT");
			dlgAlert.setPositiveButton("OK", null);
		}
	}


	void findBT() {
		mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
		if (mBluetoothAdapter == null) {
			WriteLog("No bluetooth adapter available");
		}

		if (!mBluetoothAdapter.isEnabled()) {
			Intent enableBluetooth = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
			startActivityForResult(enableBluetooth, 0);
		}

		Set<BluetoothDevice> pairedDevices = mBluetoothAdapter.getBondedDevices();
		if (pairedDevices.size() > 0) {
			for (BluetoothDevice device : pairedDevices) {
				if (device.getName().equals("HC-06")) {
					mmDevice = device;
					WriteLog("Bluetooth Device Found");
					break;
				}
			}
		}

	}

	void openBT() throws IOException {

		//DONT CHANGE, THIS DEFINES THE BLUETOOTH SERIAL PROTOCOL
		UUID uuid = UUID.fromString("00001101-0000-1000-8000-00805F9B34FB");

		mmSocket = mmDevice.createRfcommSocketToServiceRecord(uuid);
		WriteLog("Initializing Socket... Please wait");
		mmSocket.connect();

		//mmSocket.connect();
		WriteLog("Socket connected!");
		mmOutputStream = mmSocket.getOutputStream();
		mmInputStream = mmSocket.getInputStream();

		WriteLog("Listening..");


		beginListenForData();

		WriteLog("Bluetooth Opened");
	}

	void beginListenForData() {
		final Handler handler = new Handler();
		final byte delimiter = 10; //This is the ASCII code for a newline character

		stopWorker = false;
		readBufferPosition = 0;
		readBuffer = new byte[1024];
		workerThread = new Thread(new Runnable() {
			public void run() {
				while (!Thread.currentThread().isInterrupted() && !stopWorker) {
					try {
						int bytesAvailable = mmInputStream.available();
						if (bytesAvailable > 0) {
							byte[] packetBytes = new byte[bytesAvailable];
							mmInputStream.read(packetBytes);
							for (int i = 0; i < bytesAvailable; i++) {
								byte b = packetBytes[i];
								if (b == delimiter) {
									byte[] encodedBytes = new byte[readBufferPosition];
									System.arraycopy(readBuffer, 0, encodedBytes, 0, encodedBytes.length);
									final String data = new String(encodedBytes, "US-ASCII");
									readBufferPosition = 0;

									handler.post(new Runnable() {
										public void run() {
											loadJson(data);
										}
									});

								} else {
									readBuffer[readBufferPosition++] = b;
								}
							}
						}
					} catch (IOException ex) {
						stopWorker = true;
					}
				}
			}
		});

		workerThread.start();
	}

	public void WriteLog(String str) {
		android.util.Log.println(android.util.Log.VERBOSE, "info", str);
	}

	void SetTextFromPID(TextView v, String PID, JSONObject j) {
		if (j.has(PID)) {
			try {
				v.setText(String.valueOf(j.get(PID)));
			} catch (JSONException e) {
				WriteLog(e.toString());
			}
		} else {
			v.setText("Null");
		}
	}


	void loadJson(String json) {

		try {
			JSONObject j = new JSONObject(json);

			SetTextFromPID(time, "DeltaTime", j);
			SetTextFromPID(rpm, "Rpm", j);
			SetTextFromPID(kmh, "Speed", j);
			SetTextFromPID(lat, "Lat", j);
			SetTextFromPID(lng, "Lng", j);
			SetTextFromPID(date, "Date", j);
			SetTextFromPID(time, "Time", j);

			if (loggingToggleBtn.isChecked()) {
				jsonArray.put(j);
				arrayCount.setText(String.valueOf(jsonArray.length() - 1));
			}

		} catch (JSONException e) {
			android.util.Log.println(android.util.Log.VERBOSE, "error", e.toString());
		}
	}

	void sendData() throws IOException {
		String msg = myTextbox.getText().toString();
		msg += "\n";
		mmOutputStream.write(msg.getBytes());
		WriteLog("Data Sent");
	}

	void closeBT() throws IOException {
		stopWorker = true;
		mmOutputStream.close();
		mmInputStream.close();
		mmSocket.close();
		WriteLog("Bluetooth Closed");
	}
}