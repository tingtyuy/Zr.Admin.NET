using ZR.Model.Business.Dto;
using ZR.Model.Business;

namespace ZR.Service.Business.IBusinessService
{
    /// <summary>
    /// service接口
    /// </summary>
    public interface ICompanyService : IBaseService<Company>
    {
        PagedInfo<CompanyDto> GetList(CompanyQueryDto parm);

        Company GetInfo(int Id);


        Company AddCompany(Company parm);
        int UpdateCompany(Company parm);


    }
}
