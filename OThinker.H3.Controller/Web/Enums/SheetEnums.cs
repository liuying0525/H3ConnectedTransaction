using System;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// �򿪱���ģʽ
    /// </summary>
    public enum SheetMode
    {
        /// <summary>
        /// δָ��
        /// </summary>
        Unspecified = InteractiveType.Unspecified,
        /// <summary>
        /// ��д��������û�����
        /// </summary>
        Work = InteractiveType.Work,
        /// <summary>
        /// �鿴ĳ������ʵ���ı�
        /// </summary>
        View = InteractiveType.View,
        /// <summary>
        /// ��������
        /// </summary>
        Originate = InteractiveType.Create,
        /// <summary>
        /// ��ӡģʽ
        /// </summary>
        Print = InteractiveType.Print,
        /// <summary>
        /// ���Ĳ鿴��ģʽ(����View��ԭ����Item�Ӳ�ͬ�ӿڻ�ȡ)
        /// </summary>
        Circulate = InteractiveType.Circulate
    }

    /// <summary>
    /// ��ģʽ
    /// </summary>
    public enum SheetDataType
    {
        /// <summary>
        /// δָ��
        /// </summary>
        Unspecified = 0,
        /// <summary>
        /// ��������
        /// </summary>
        Workflow = 1,
        /// <summary>
        /// ҵ�����
        /// </summary>
        BizObject = 2
    }

    /// <summary>
    /// �����򿪷�ʽ
    /// </summary>
    public enum AttachmentOpenMethod
    {
        /// <summary>
        /// ���ط�ʽ
        /// </summary>
        Download = 0,
        /// <summary>
        /// �����ֱ�Ӵ򿪷�ʽ
        /// </summary>
        Browse = 1,
        /// <summary>
        /// ʹ�� NTKO �ķ�ʽ�򿪸���
        /// </summary>
        NTKO = 2
    }

}