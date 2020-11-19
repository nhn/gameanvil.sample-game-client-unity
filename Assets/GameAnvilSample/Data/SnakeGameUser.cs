using System.Collections.Generic;

// 게임 유저 데이터 객체
public class SnakeGameUser
{

    public string Id { get; set; }
    public string Nickname { get; set; }
    public long Score { get; set; }

    public List<SnakePositionInfo> PositionList { get; set; }
}
