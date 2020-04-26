using System;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// Ŀ���Ĳ��뷽ʽ
    /// </summary>
    public enum DestParticipantType
    {
        /// <summary>
        /// ʹ��Ĭ�ϵ�
        /// </summary>
        Default = 0,
        /// <summary>
        /// ʹ��Ŀ������һ�ε�ȫ��������
        /// </summary>
        AllLastParticipants = 1,
        /// <summary>
        /// ���п�ѡ�Ĳ�����
        /// </summary>
        AllOptional = 2
    }

    /// <summary>
    /// ���ύ�¼���
    /// </summary>
    public class SheetSubmitEventArgs
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="ParticipateJob">��ʲô��ɫ����ύ</param>
        /// <param name="ButtonType">�������¼��İ�ť������</param>
        /// <param name="EventType">�������¼��İ�ť��Ӧ���¼�����</param>
        /// <param name="Finish">���¼��Ƿ�������¼������ύ/��ת���¼�����������¼��������治������¼�</param>
        /// <param name="DestActivityCode">Ҫ��ת����һ������ƣ�����ΪNull����ʾ��ϵͳ�Զ�ָ��</param>
        /// <param name="DestParticipantType">���ָ����һ��������ƣ����������Ч����ʾ��һ����Ĳ������Ƿ�ʹ��ǰһ�εĻ�����ߡ�����A->B��A�ǰһ�εĲ�������(a1, a2, a3)�������B��ص�A�����Ҹò���ΪTrue�����ʾA�Ĳ������Զ�ʹ��(a1, a2, a3)</param>
        /// <param name="ActionName">���¼�������������ΪNull</param>
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
        /// ���ύ�Ƿ���Ҫ��ɵ�ǰ������
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
        /// Ŀ������
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
        /// ��ת��Ŀ����ʱ��ѡ������ߵķ�ʽ
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
        /// ��ȡ�������û������ɫ
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
        /// ��������
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
        /// ��ť������
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
        /// ��ť������
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
    /// ���ύ�¼�
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="e"></param>
    public delegate void SheetSubmitEventHandler(object Sender, SheetSubmitEventArgs e);
}
