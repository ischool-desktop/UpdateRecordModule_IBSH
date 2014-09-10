using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.DocumentValidator;

namespace UpdateRecordModule_IBSH.ValidationRule
{
    public class CounselRowValidatorFactory : IRowValidatorFactory
    {
        #region IRowValidatorFactory 成員

        IRowVaildator IRowValidatorFactory.CreateRowValidator(string typeName, System.Xml.XmlElement validatorDescription)
        {
            switch (typeName.ToUpper())
            {
                case "STUDCHECKSTUDENTNUMBERSTATUSVAL":
                    return new RowValidator.StudCheckStudentNumberStatusVal();
                case "STUDCHECKUPDATERECORDVAL01":
                    return new RowValidator.StudCheckUpdateRecordVal01();
                default:
                    return null;
            }
        }

        #endregion
    }
}
