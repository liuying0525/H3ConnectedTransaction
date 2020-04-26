using System;

namespace OThinker.H3.Controllers
{
	/// <summary>
	/// ButtonType ��ժҪ˵����
	/// </summary>
    public enum SheetButtonEventType
    {
        /// <summary>
        /// �������κ��¼�
        /// </summary>
        None = 0,
        /// <summary>
        /// ���°�ť
        /// </summary>
        UpdateButton = 1,
        /// <summary>
        /// ��ɰ�ť
        /// </summary>
        FinishButton = WorkItem.ActionEventType.Forward, 
        /// <summary>
        /// �������̻��˵��¼�
        /// </summary>
        ReturnButton = WorkItem.ActionEventType.Backward
    }
}
