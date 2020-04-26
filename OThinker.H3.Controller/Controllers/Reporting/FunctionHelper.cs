using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.Controllers.Reporting
{
    public class FunctionHelper
    {
        public FunctionHelper() { }
        public FunctionHelper(string Name, FunctionCategory[] FunctionCategories, FunctionScenarioType[] FunctionScenarioTypes, string Description, string Example)
        {
            this._Name = Name;
            this._DisplayName = DisplayName;
            this._FunctionCategories = FunctionCategories;
            this._FunctionScenarioTypes = FunctionScenarioTypes;
            this._Description = Description;
            this._Example = Example;
        }

        public FunctionHelper(string Name, string DisplayName, FunctionCategory[] FunctionCategorys, FunctionScenarioType[] FunctionScenarioTypes, string Description, string Example)
        {
            this._Name = Name;
            this._DisplayName = DisplayName;
            this._FunctionCategories = FunctionCategories;
            this._FunctionScenarioTypes = FunctionScenarioTypes;
            this._Description = Description;
            this._Example = Example;

        }

        private string _DisplayName;
        public string DisplayName
        {
            get
            {
                return this._DisplayName;
            }
            set { _DisplayName = value; }
        }


        public bool SupportCategory(FunctionScenarioType type, FunctionCategory category)
        {
            if (type == FunctionScenarioType.Unspecified)
            {
                return true;
            }
            FunctionCategory[] categories = null;
            switch (type)
            {
                case FunctionScenarioType.Unspecified:
                    categories = new FunctionCategory[]{
                  };
                    break;
                case FunctionScenarioType.Router:
                    categories = new FunctionCategory[]{
                  FunctionCategory.Math,
                  FunctionCategory.Text,
                  FunctionCategory.Time,
                  FunctionCategory.Logic,
                  FunctionCategory.Organization,
                  FunctionCategory.Other
                  };
                    break;
                case FunctionScenarioType.Computation:
                    categories = new FunctionCategory[]{
                  FunctionCategory.Math,
                  FunctionCategory.Text,
                  FunctionCategory.Time,
                  FunctionCategory.Logic,           
                  FunctionCategory.Other
                  };
                    break;
                case FunctionScenarioType.Submit:
                    categories = new FunctionCategory[]{
                  FunctionCategory.Math,
                  FunctionCategory.Text,
                  FunctionCategory.Time,
                  FunctionCategory.Logic,
                  FunctionCategory.Organization, 
                  FunctionCategory.Other
                  };
                    break;
                case FunctionScenarioType.Business:
                    categories = new FunctionCategory[]{
                  FunctionCategory.Math,
                  FunctionCategory.Time,
                  FunctionCategory.Logic,
                  FunctionCategory.BackMethod, 
                  FunctionCategory.Other
                  };
                    break;
                case FunctionScenarioType.AssociationFilter:
                    categories = new FunctionCategory[]{
                  FunctionCategory.Math,
                  FunctionCategory.Text,
                  FunctionCategory.Time,
                  FunctionCategory.Logic,
                  FunctionCategory.Other
                  };
                    break;
                case FunctionScenarioType.ReportSource:
                    categories = new FunctionCategory[]{
                  FunctionCategory.Time,
                  FunctionCategory.Logic
                  };
                    break;
                default:
                    categories = new FunctionCategory[] { };
                    break;
            }
            return categories.Contains(category);
        }

        public bool SupportScenario(FunctionScenarioType type)
        {
            if (type == FunctionScenarioType.Unspecified)
            {
                return true;
            }
            FunctionScenarioType[] types = this.FunctionScenarioTypes;
            if (types == null)
            {
                return false;
            }
            foreach (FunctionScenarioType t in types)
            {
                if (type == t)
                {
                    return true;
                }
            }
            return false;
        }
        private string _Name;
        public string Name
        {
            get { return this._Name; }
            set { _Name = value; }
        }


        private FunctionScenarioType[] _FunctionScenarioTypes;
        public FunctionScenarioType[] FunctionScenarioTypes
        {
            get { return _FunctionScenarioTypes; }
            set { _FunctionScenarioTypes = value; }
        }
        private FunctionCategory[] _FunctionCategories;
        public FunctionCategory[] FunctionCategories
        {
            get { return _FunctionCategories; }
            set { _FunctionCategories = value; }
        }

        private string _Example;
        public string Example
        {
            get
            {
                return this._Example;
            }
            set { _Example = value; }
        }

        private string _Description;
        public string Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                _Description = value;
            }
        }
    }



}
