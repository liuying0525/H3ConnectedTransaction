using DongZheng.H3.WebApi;


/// <summary>
/// 资产信息
/// </summary>
public class VEHICLE
{
    /// <summary>
    /// 制造商Code
    /// </summary>
    public string asset_make_cde { get; set; }
    /// <summary>
    /// 制造商名称
    /// </summary>
    public string asset_make_dsc { get; set; }
    /// <summary>
    /// 车型Code
    /// </summary>
    public string asset_brand_cde { get; set; }
    /// <summary>
    /// 车型名称
    /// </summary>
    public string asset_brand_dsc { get; set; }
    /// <summary>
    /// 型号Code
    /// </summary>
    public string asset_model_dsc { get; set; }
    /// <summary>
    /// 型号名称
    /// </summary>
    public string asset_model_cde { get; set; }
    /// <summary>
    /// 发动机型号ID
    /// </summary>
    public string miocn_id { get; set; }
    /// <summary>
    /// 发动机型号编号
    /// </summary>
    public string miocn_nbr { get; set; }
    /// <summary>
    /// 发动机型号名称
    /// </summary>
    public string miocn_dsc { get; set; }
    /// <summary>
    /// 资产价格
    /// </summary>
    public string retail_price_amt { get; set; }
    /// <summary>
    /// 类型Code
    /// </summary>
    public string vehicle_type_cde { get; set; }
    /// <summary>
    /// 类型名称
    /// </summary>
    public string vehicle_type_dsc { get; set; }
    /// <summary>
    /// 子类型Code
    /// </summary>
    public string vehicle_subtyp_cde { get; set; }
    /// <summary>
    /// 子类型名称
    /// </summary>
    public string vehicle_subtyp_dsc { get; set; }
    /// <summary>
    /// 变速器
    /// </summary>
    public string transmission_type_cde { get; set; }
}

public class CAP_EXPOSURE
{
    public string NET_FINANCED_AMT { get; set; }
    public string EXP_APPLICATION_NUMBER { get; set; }
    public string APPLICATION_STATUS_CDE { get; set; }
    public string APPLICATION_STATUS_DSC { get { return Proposal.GetCAPStateName(APPLICATION_STATUS_CDE); } }
    public string EXP_APPLICANT_ROLE_IND { get; set; }

    public string EXP_APPLICANT_ROLE_NAME { get { return Proposal.GetCAPRoleName(EXP_APPLICANT_ROLE_IND); } }
    public string EXP_APPLICANT_CARD_ID { get; set; }
    public string EXP_APPLICANT_NAME { get; set; }
    public string FP_GROUP_NME { get; set; }
    public string APPLICATION_STATUS_DTE { get; set; }

    public string NO_OF_TERMS { get; set; }
    public string ASSET_MAKE_DSC { get; set; }
    public string ASSET_MODEL_DSC { get; set; }
    public string ASSET_BRAND_DSC { get; set; }
    public string TYPE { get; set; }
    /// <summary>
    /// 角色（B：借款人，C：共借人，G：担保人）
    /// </summary>
    public string APPLICANT_ROLE_IND { get; set; }
    /// <summary>
    /// 是否可以勾选：T表示不可编辑，其它的可编辑
    /// </summary>
    public string SYS_IDENTIFIED_IND { get; set; }
    /// <summary>
    /// 是否默认勾选：T表示默认勾选 并计算敞口值
    /// </summary>
    public string IS_EXPOSED_IND { get; set; }
    /// <summary>
    /// 是否算负债的标识:T表示算负债;
    /// </summary>
    public string IS_CONTINGENT_IND { get; set; }
}

public class CMS_EXPOSURE
{
    public string PRINCIPLE_OUTSTANDING_AMT { get; set; }
    public string EXP_APPLICATION_NUMBER { get; set; }
    public string CONTRACT_NUMBER { get; set; }
    public string REQUEST_STATUS_DSC { get; set; }
    public string CONTRACT_STATUS_CDE { get; set; }
    public string EXP_INACTIVE_IND { get; set; }
    public string EXP_INACTIVE_DSC { get { return EXP_INACTIVE_IND == "F" ? "Active" : "Inactive"; } }

    public string EXP_APPLICANT_ROLE_IND { get; set; }
    public string EXP_APPLICANT_ROLE_NAME { get { return Proposal.GetCAPRoleName(EXP_APPLICANT_ROLE_IND); } }
    public string EXP_APPLICANT_CARD_ID { get; set; }
    public string EXP_APPLICANT_NAME { get; set; }
    public string BUSINESS_PARTNER_NME { get; set; }
    public string FP_GROUP_NME { get; set; }

    public string CONTRACT_DTE { get; set; }
    public string INTEREST_RATE { get; set; }
    public string NET_FINANCED_AMT { get; set; }
    public string ASSET_MAKE_DSC { get; set; }
    public string ASSET_MODEL_DSC { get; set; }

    public string ASSET_BRAND_DSC { get; set; }
    public string OVERDUE_30_DAYS { get; set; }
    public string OVERDUE_60_DAYS { get; set; }
    public string OVERDUE_90_DAYS { get; set; }
    public string OVERDUE_ABOVE_90_DAYS { get; set; }

    public string OVERDUE_ABOVE_120_DAYS { get; set; }
    public string NO_OF_TERMS { get; set; }
    public string NO_OF_TERMS_PAID { get; set; }

    /// <summary>
    /// 角色（B：借款人，C：共借人，G：担保人）
    /// </summary>
    public string APPLICANT_ROLE_IND { get; set; }
    /// <summary>
    /// 是否可以勾选：T表示不可编辑，其它的可编辑
    /// </summary>
    public string SYS_IDENTIFIED_IND { get; set; }
    /// <summary>
    /// 是否默认勾选：T表示默认勾选 并计算敞口值
    /// </summary>
    public string IS_EXPOSED_IND { get; set; }
    /// <summary>
    /// 是否算负债的标识:T表示算负债;
    /// </summary>
    public string IS_CONTINGENT_IND { get; set; }
}

public class rtn_data
{
    public int code { get; set; }
    public object data { get; set; }
    public string message { get; set; } 
}

public class ResponseData<T>
{
    public string Code { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public bool Success { get; set; }
}

public class CustomSettingParas
{
    public string SysStopFromTime { get; set; }
    public string SysStopToTime { get; set; }
    public string SysStopMessage { get; set; }

    public string HttpPostCreatedURL { get; set; }
    public string HttpPostUpdatedURL { get; set; }
    public string HttpPostRemovedURL { get; set; }
}
