using System;

public interface IUI
{
    Enum GetId();
}

public enum UISystemType
{
    Waiting,            // 입력 차단을 위한 특수 UI
    PopupNotice,        // 확인버튼
    PopupYesOrNo,       // 예, 아니오 버튼

    End,
}

