using UnityEngine;

// Tardis connector 
namespace TardisConnector
{
    public class ConnectHandler : MonoBehaviour
    {
        private static Tardis.Connector connector = null;
        public static Tardis.Connector Instance
        {
            get
            {
                return connector;
            }
        }
        
        [SerializeField]
        public Tardis.Connector.Config config;
        private void Awake()
        {
            if (connector != null)
            {
                Destroy(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
            connector = new Tardis.Connector(config);
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
            Tardis.ProtocolManager.getInstance().RegisterProtocol(0, Com.Nhn.Tardis.Sample.Protocol.AuthenticationReflection.Descriptor);
            Tardis.ProtocolManager.getInstance().RegisterProtocol(1, Com.Nhn.Tardis.Sample.Protocol.GameMultiReflection.Descriptor);
            Tardis.ProtocolManager.getInstance().RegisterProtocol(2, Com.Nhn.Tardis.Sample.Protocol.GameSingleReflection.Descriptor);
            Tardis.ProtocolManager.getInstance().RegisterProtocol(3, Com.Nhn.Tardis.Sample.Protocol.ResultReflection.Descriptor);
            Tardis.ProtocolManager.getInstance().RegisterProtocol(4, Com.Nhn.Tardis.Sample.Protocol.UserReflection.Descriptor);
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