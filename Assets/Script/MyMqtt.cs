

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;

namespace ThisIsMine
{

    public class data_ss
    {
        public string ss_name { get; set; }
        public string ss_unit { get; set; }
        public string ss_value { get; set; }
    }
    public class Status_Data
    {

        public int temperature { get; set; }
        public int humidity { get; set; }
        //public string led_on { get; set; }     //0: off; 1: on
        //public string pump_on { get; set; }    //0: off; 1: on

    }

    public class Led_Data
    {
        public string device { get; set; }
        public string status { get; set; }     //"ON", "OFF"
    }

    public class Pump_Data
    {
        public string device { get; set; }
        public string status { get; set; }     // "ON", "OFF"
    }

    public class Config_Data
    {
        public float temperature_max { get; set; }
        public float temperature_min { get; set; }
        public int mode_pump_auto { get; set; }
    }

    public class MyMqtt : M2MqttUnityClient
    {
        //List to subscribe
        public List<string> topics = new List<string>();
        public string msg_received_from_topic_status = "";
        public string msg_received_from_topic_control = "";
        public Toggle ledToggle;
        public Toggle pumpToggle;

        private List<string> eventMessages = new List<string>();
        [SerializeField]
        public Status_Data _status_data;
        public Pump_Data _pump_data;
        public Led_Data _led_data;

        [SerializeField]
        private InputField broker_inp;
        [SerializeField]
        private InputField user_name_inp;
        [SerializeField]
        private InputField password_inp;


        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        public void OnClickButtonConnect()
        {
            brokerAddress = broker_inp.text;
            mqttUserName = user_name_inp.text;
            mqttPassword = password_inp.text;
            Connect();
        }

        public void OnClickButtonDisconnect()
        {
            Disconnect();
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            SubscribeTopics();
            Debug.Log("Connected");
            this.GetComponent<MyManager>().ShowInfoPanel();
        }

        protected override void SubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                }
            }
        }

        protected override void UnsubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Unsubscribe(new string[] { topic });
                }
            }
        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            Debug.Log("CONNECTION FAILED! " + errorMessage);
        }

        protected override void OnDisconnected()
        {
            Debug.Log("Disconnected.");
            this.GetComponent<MyManager>().ShowLoginPanel();
        }

        protected override void OnConnectionLost()
        {
            Debug.Log("CONNECTION LOST!");
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log("Received: " + msg);
            //StoreMessage(msg);

            if (topic == topics[0])
                ProcessMessageStatus(msg);


            
            if (topic == topics[1])
                ProcessLedControl(msg);

            
            
            if (topic == topics[2])
                ProcessPumpControl(msg);
            

        }

        private void ProcessMessageStatus(string msg)
        {
            _status_data = JsonConvert.DeserializeObject<Status_Data>(msg);
            msg_received_from_topic_status = msg;
            GetComponent<MyManager>().Update_Status(_status_data);
        }

        private void ProcessLedControl(string msg)
        {
            _led_data = JsonConvert.DeserializeObject<Led_Data>(msg);
            msg_received_from_topic_status = msg;
            GetComponent<MyManager>().Update_Led(_led_data);
        }
        
        private void ProcessPumpControl(string msg) //Recive (Subcribe)
        {
            _pump_data = JsonConvert.DeserializeObject<Pump_Data>(msg);
            msg_received_from_topic_status = msg;
            GetComponent<MyManager>().Update_Pump(_pump_data);
        }

        public void PublishLed()        //Send
        {
            ledToggle.interactable = false;

            //Send data
            string data = "{\"device\":\"LED\"," + (ledToggle.isOn ? "\"status\": \"ON\"}" : "\"status\": \"OFF\"}");
            client.Publish(topics[1], System.Text.Encoding.UTF8.GetBytes(data), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log("Have published" + data);
        }

        public void PublishPump()        //Send
        {
            pumpToggle.interactable = false;

            //Send data
            string data = "{\"device\":\"PUMP\"," + (pumpToggle.isOn ? "\"status\": \"ON\"}" : "\"status\": \"OFF\"}");
            client.Publish(topics[2], System.Text.Encoding.UTF8.GetBytes(data), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log("Have published" + data);
        }
        /*
        private void ProcessPumpStatus(string msg)
        {
            _pump_data = JsonConvert.DeserializeObject<Pump_Data>(msg);
            msg_received_from_topic_status = msg;
            GetComponent<MyManager>().Update_Pump(_pump_data);
        }
        */

        private void ProcessMessageControl(string msg)
        {
            //_controlFan_data = JsonConvert.DeserializeObject<ControlFan_Data>(msg);
            //msg_received_from_topic_control = msg;
            //GetComponent<ChuongGaManager>().Update_Control(_controlFan_data);

        }
        // Publish state of led

        //Publish state of pump
        public void Update_Pump(Pump_Data pump_Data)
        {
            //Not allowed user to update the data until get the respnose from server
            pumpToggle.interactable = false;

            //Send data
            string data = "{\"device\":\"PUMP\"," + (pumpToggle.isOn ? "\"status\": \"ON\"}" : "\"status\": \"OFF\"}");
            client.Publish(topics[2], System.Text.Encoding.UTF8.GetBytes(data), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log("Have published" + data);
        }

        private void OnDestroy()
        {
            Disconnect();
        }
    }

}


/*
 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;

namespace ChuongGa
{
    public class Status_Data
    {
        public string project_id { get; set; }
        public string project_name { get; set; }
        public string station_id { get; set; }
        public string station_name { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string volt_battery { get; set; }
        public string volt_solar { get; set; }
        public List<data_ss> data_ss { get; set; }
        public string device_status { get; set; }
    }

    public class data_ss
    {
        public string ss_name { get; set; }
        public string ss_unit { get; set; }
        public string ss_value { get; set; }
    }

    public class Config_Data
    {
        public float temperature_max { get; set; }
        public float temperature_min { get; set; }
        public int mode_fan_auto { get; set; }
    }

    public class ControlFan_Data
    {
        public int fan_status { get; set; }
        public int device_status { get; set; }

    }

    public class ChuongGaMqtt : M2MqttUnityClient
    {
        public List<string> topics = new List<string>();


        public string msg_received_from_topic_status = "";
        public string msg_received_from_topic_control = "";


        private List<string> eventMessages = new List<string>();
        [SerializeField]
        public Status_Data _status_data;
        [SerializeField]
        public Config_Data _config_data;
        [SerializeField]
        public ControlFan_Data _controlFan_data;
        


        public void PublishConfig()
        {
            _config_data = new Config_Data();
            GetComponent<ChuongGaManager>().Update_Config_Value(_config_data);
            string msg_config = JsonConvert.SerializeObject(_config_data);
            client.Publish(topics[1], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("publish config");
        }

        public void PublishFan()
        {
            _controlFan_data = GetComponent<ChuongGaManager>().Update_ControlFan_Value(_controlFan_data);
            string msg_config = JsonConvert.SerializeObject(_controlFan_data);
            client.Publish(topics[2], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("publish fan");


        }

        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }


        protected override void OnConnecting()
        {
            base.OnConnecting();
            //SetUiMessage("Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
        }

        protected override void OnConnected()
        {
            base.OnConnected();

            SubscribeTopics();
        }

        protected override void SubscribeTopics()
        {

            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

                }
            }
        }

        protected override void UnsubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Unsubscribe(new string[] { topic });
                }
            }

        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            Debug.Log("CONNECTION FAILED! " + errorMessage);
        }

        protected override void OnDisconnected()
        {
            Debug.Log("Disconnected.");
        }

        protected override void OnConnectionLost()
        {
            Debug.Log("CONNECTION LOST!");
        }



        protected override void Start()
        {

            base.Start();
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log("Received: " + msg);
            //StoreMessage(msg);
            if (topic == topics[0])
                ProcessMessageStatus(msg);

            if (topic == topics[2])
                ProcessMessageControl(msg);
        }

        private void ProcessMessageStatus(string msg)
        {
             _status_data = JsonConvert.DeserializeObject<Status_Data>(msg);
            msg_received_from_topic_status = msg;
            GetComponent<ChuongGaManager>().Update_Status(_status_data);

        }

        private void ProcessMessageControl(string msg)
        {
            _controlFan_data = JsonConvert.DeserializeObject<ControlFan_Data>(msg);
            msg_received_from_topic_control = msg;
            GetComponent<ChuongGaManager>().Update_Control(_controlFan_data);

        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnValidate()
        {
            //if (autoTest)
            //{
            //    autoConnect = true;
            //}
        }

        public void UpdateConfig()
        {
           
        }

        public void UpdateControl()
        {

        }
    }
}
 */