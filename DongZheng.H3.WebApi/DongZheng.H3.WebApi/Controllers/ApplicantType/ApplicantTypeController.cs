using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.ApplicantType
{
    [ValidateInput(false)]
    [Xss]
    public class ApplicantTypeController : OThinker.H3.Controllers.ControllerBase
    {
        // GET: ApplicantType
        public string Index()
        {
            return "Workitem ApplicantType";
        }

        public override string FunctionCode
        {
            get { return ""; }
        }

        #region 
        /// <summary>
        /// 获取人员关系
        /// </summary>
        public JsonResult GetPersonnelRelationshipResult(string objectid)
        {
            List<object> objs = new List<object>();
            string sql = "select applicanttype.MAIN_APPLICANT,applicanttype.IDENTIFICATION_CODE1,applicanttype.APPLICANT_TYPE,applicanttype.GUARANTOR_TYPE, applicantdetail.FIRST_THI_NME,applicantdetail.ID_CARD_NBR,applicantdetail.EMAIL_ADDRESS from I_APPLICANT_TYPE applicanttype, I_APPLICANT_Detail applicantdetail where applicanttype.PARENTOBJECTID = applicantdetail.PARENTOBJECTID and applicanttype.IDENTIFICATION_CODE1 = applicantdetail.IDENTIFICATION_CODE2 and applicanttype.OBJECTID = '"+ objectid + "'";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                objs.Add(new
                {
                    main_applicant = dt.Rows[0]["MAIN_APPLICANT"].ToString(),
                    identification_code1 = dt.Rows[0]["IDENTIFICATION_CODE1"].ToString(),
                    applicant_type = dt.Rows[0]["APPLICANT_TYPE"].ToString(),
                    guarantor_type = dt.Rows[0]["GUARANTOR_TYPE"].ToString(),
                    first_thi_nme = dt.Rows[0]["FIRST_THI_NME"].ToString(),
                    id_card_nbr = dt.Rows[0]["ID_CARD_NBR"].ToString(),
                    email_address = dt.Rows[0]["EMAIL_ADDRESS"].ToString(),
                });
            }
            return Json(new { Success = true, Data = objs }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}