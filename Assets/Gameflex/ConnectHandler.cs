using UnityEngine;

// Gameflex connector 
namespace GameflexConnector
{
    public class ConnectHandler : MonoBehaviour
    {
        private static Gameflex.Connector connector = null;
        public static Gameflex.Connector Instance
        {
            get
            {
                return connector;
            }
        }
        
        [SerializeField]
        public Gameflex.Connector.Config config;
        private void Awake()
        {
            if (connector != null)
            {
                Destroy(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
            connector = new Gameflex.Connector(config);
            // 커넥터 로그 추가
            connector.Logger += (level, log) =>
            {
                Debug.Log(string.Format("Log[{0}]:{1}", level, log));
            };
            connector.LvNetLogger += (level, log) =>
            {
                Debug.Log(string.Format("Net[{0}]:{1}", level, log));
            };

            // 서버와 같은 순서로 프로토콜 등록
            Gameflex.ProtocolManager.getInstance().RegisterProtocol(0, Com.Nhn.Gameflex.Sample.Protocol.AuthenticationReflection.Descriptor);
            Gameflex.ProtocolManager.getInstance().RegisterProtocol(1, Com.Nhn.Gameflex.Sample.Protocol.GameMultiReflection.Descriptor);
            Gameflex.ProtocolManager.getInstance().RegisterProtocol(2, Com.Nhn.Gameflex.Sample.Protocol.GameSingleReflection.Descriptor);
            Gameflex.ProtocolManager.getInstance().RegisterProtocol(3, Com.Nhn.Gameflex.Sample.Protocol.ResultReflection.Descriptor);
            Gameflex.ProtocolManager.getInstance().RegisterProtocol(4, Com.Nhn.Gameflex.Sample.Protocol.UserReflection.Descriptor);
        }

        private void Update()
        {
            connector.Update();
        }

        private void OnDisable()
        {
            if (connector.IsConnected())
            {
                connector.CloseSocket();
            }
        }
    }
}