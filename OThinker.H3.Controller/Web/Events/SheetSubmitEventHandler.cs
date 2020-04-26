using System;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 目标活动的参与方式
    /// </summary>
    public enum DestParticipantType
    {
        /// <summary>
        /// 使用默认的
        /// </summary>
        Default = 0,
        /// <summary>
        /// 使用目标活动的上一次的全部参与者
        /// </summary>
        AllLastParticipants = 1,
        /// <summary>
        /// 所有可选的参与者
        /// </summary>
        AllOptional = 2
    }

    /// <summary>
    /// 表单提交事件。
    /// </summary>
    public class SheetSubmitEventArgs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ParticipateJob">以什么角色身份提交</param>
        /// <param name="ButtonType">触发该事件的按钮的类型</param>
        /// <param name="EventType">触发该事件的按钮对应的事件类型</param>
        /// <param name="Finish">该事件是否是完成事件，表单提交/跳转等事件都属于完成事件，表单保存不是完成事件</param>
        /// <param name="DestActivityCode">要跳转的下一个活动名称，可以为Null，表示由系统自动指定</param>
        /// <param name="DestParticipantType">如果指定下一个活动的名称，这个参数有效，表示下一个活动的参与者是否使用前一次的活动参与者。比如A->B，A活动前一次的参与者是(a1, a2, a3)，如果从B打回到A，并且该参数为True，则表示A的参与者自动使用(a1, a2, a3)</param>
        /// <param name="ActionName">该事件的描述，可以为Null</param>
        public SheetSubmitEventArgs(
            string ParticipateJob,
            SheetButtonType ButtonType,
            SheetButtonEventType EventType,
            bool Finish,
            string DestActivityCode,
            DestParticipantType DestParticipantType,
            string ActionName)
        {
            this._ParticipateJob = ParticipateJob;
            this._ButtonType = ButtonType;
            this._EventType = EventType;
            this._Finish = Finish;
            this._DestActivityCode = DestActivityCode;
            this._DestParticipantType = DestParticipantType;
            this._ActionName = ActionName;
        }

        private bool _Finish = false;
        /// <summary>
        /// 该提交是否是要完成当前工作项
        /// </summary>
        public bool Finish
        {
            get
            {
                return this._Finish;
            }
            set
            {
                this._Finish = value;
            }
        }

        private string _DestActivityCode;
        /// <summary>
        /// 目标活动名称
        /// </summary>
        public string DestActivityCode
        {
            get
            {
                return this._DestActivityCode;
            }
            set
            {
                this._DestActivityCode = value;
            }
        }

        private DestParticipantType _DestParticipantType;
        /// <summary>
        /// 跳转到目标活动的时候，选择参与者的方式
        /// </summary>
        public DestParticipantType DestParticipantType
        {
            get
            {
                return this._DestParticipantType;
            }
            set
            {
                this._DestParticipantType = value;
            }
        }

        private string _ParticipateJob = null;
        /// <summary>
        /// 获取或设置用户参与角色
        /// </summary>
        public string ParticipateJob
        {
            get
            {
                return this._ParticipateJob;
            }
            set
            {
                this._ParticipateJob = value;
            }
        }

        private string _ActionName;
        /// <summary>
        /// 操作描述
        /// </summary>
        public string ActionName
        {
            get
            {
                return this._ActionName;
            }
            set
            {
                this._ActionName = value;
            }
        }

        private SheetButtonType _ButtonType;
        /// <summary>
        /// 按钮的类型
        /// </summary>
        public SheetButtonType ButtonType
        {
            get
            {
                return this._ButtonType;
            }
            set
            {
                this._ButtonType = value;
            }
        }

        private SheetButtonEventType _EventType;
        /// <summary>
        /// 按钮的类型
        /// </summary>
        public SheetButtonEventType EventType
        {
            get
            {
                return this._EventType;
            }
            set
            {
                this._EventType = value;
            }
        }
    }

    /// <summary>
    /// 表单提交事件
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="e"></param>
    public delegate void SheetSubmitEventHandler(object Sender, SheetSubmitEventArgs e);
}
