﻿using cip_blue;
using cip_blue.Models;
using Events;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Throw;
using System.Text.Json;
using System.Windows.Shapes;
using NLog;
//using Meziantou.Framework.WPF.Collections;

namespace Services
{

    internal class ChartUpdateService : IService
    {
        private readonly IEventAggregator _eventAggregator;
        private  ChartData _chartData;
        private readonly ProcessDataTcp _modbusData;
        private bool isStarted;
        private readonly Random random = new();
        private Logger logger = LogManager.GetCurrentClassLogger();

        public ChartUpdateService(ProcessDataTcp modbusData, IEventAggregator eventAggregator, ChartData chartData)
        {
            _eventAggregator = eventAggregator;
            _chartData = chartData;
            _modbusData = modbusData;
            _chartData.DataPoints = new();
            _chartData.DataPoints2 = new();
      
    }
        public void DoWork()
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                string _fileName = "ChartData.json";
                string jsonString = "";

                if (!isStarted)
                {
                    using StreamReader data = File.OpenText(_fileName);
                    try
                    {

                        string readData = data.ReadToEnd();
                        //   ChartData? convertedData = JsonConvert.DeserializeObject<ChartData>(readData);

                        ChartData? convertedData = System.Text.Json.JsonSerializer.Deserialize<ChartData>(readData);
                        _chartData.DataPoints = convertedData?.DataPoints;
                        _chartData.DataPoints2 = convertedData?.DataPoints2;

                    }
                    catch (Exception ex)
                    {
                  
                        logger.Error("Error in ChartUpdate Service Load from chart json >>>> " + ex.Message);

                    }

             
                    //for (int i = 0; i < 360; i++)
                    //{

                    //    _chartData.DataPoints?.Add(new Tuple<float, double>(_modbusData.TE2, DateTime.Now.AddMinutes(i).ToOADate()));
                    //    _chartData.DataPoints2?.Add(new Tuple<float, double>(_modbusData.Tzad_pvs, DateTime.Now.AddMinutes(i).ToOADate()));
                    //}

                 
                    isStarted = true;
                    return;
                }
                else
                {
                    try
                    {
                   
                            _chartData.DataPoints?.Add(new Tuple<float, double>(_modbusData.TE2, DateTime.Now.ToOADate()));
                            if (_chartData.DataPoints.Count > 360 && _chartData.DataPoints is not null)
                                _chartData.DataPoints?.RemoveAt(0);

                   
                       
                            

                
                            _chartData.DataPoints2.Add(new Tuple<float, double>(_modbusData.Tzad_pvs, DateTime.Now.ToOADate()));
                            if (_chartData.DataPoints2.Count > 360 && _chartData.DataPoints2 is not null)
                                _chartData.DataPoints2?.RemoveAt(0);

             
                      

               //   _chartData.DataPoints2.Clear();


                        //}
                        //  _eventAggregator.GetEvent<ChartUpdateStartedEvent>().Publish();

                        string fileName = "ChartData.json";
                        string ChartDataJson = System.Text.Json.JsonSerializer.Serialize(_chartData);
                        File.WriteAllText(fileName, ChartDataJson);

                    }
                    catch ( Exception ex ) 
                    {
                       logger.Error("Error in ChartUpdate Service// delete/add  collection >>>> " + ex.Message);
                    }

                }
            });

        }
    }
}
