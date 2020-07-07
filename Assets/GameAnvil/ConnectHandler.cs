using UnityEngine;

// GameAnvil connector 
namespace GameAnvilConnector
{
    public class ConnectHandler : MonoBehaviour
    {
        private static GameAnvil.Connector connector = null;
        public static GameAnvil.Connector Instance
        {
            get
            {
                return connector;
            }
        }
        
        [SerializeField]
        public GameAnvil.Connector.Config config;
        private void Awake()
        {
            if (connector != null)
            {
                Destroy(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
            connector = new GameAnvil.Connector(config);
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
            GameAnvil.ProtocolManager.getInstance().RegisterProtocol(0, Com.Nhn.Gameanvil.Sample.Protocol.AuthenticationReflection.Descriptor);
            GameAnvil.ProtocolManager.getInstance().RegisterProtocol(1, Com.Nhn.Gameanvil.Sample.Protocol.GameMultiReflection.Descriptor);
            GameAnvil.ProtocolManager.getInstance().RegisterProtocol(2, Com.Nhn.Gameanvil.Sample.Protocol.GameSingleReflection.Descriptor);
            GameAnvil.ProtocolManager.getInstance().RegisterProtocol(3, Com.Nhn.Gameanvil.Sample.Protocol.ResultReflection.Descriptor);
            GameAnvil.ProtocolManager.getInstance().RegisterProtocol(4, Com.Nhn.Gameanvil.Sample.Protocol.UserReflection.Descriptor);
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