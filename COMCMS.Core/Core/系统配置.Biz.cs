using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using NewLife;
using NewLife.Data;
using NewLife.Log;
using NewLife.Model;
using NewLife.Reflection;
using NewLife.Threading;
using NewLife.Web;
using XCode;
using XCode.Cache;
using XCode.Configuration;
using XCode.DataAccessLayer;
using XCode.Membership;
using COMCMS.Core.Models;
using Newtonsoft.Json;

namespace COMCMS.Core
{
    /// <summary>系统配置</summary>
    public partial class Config : Entity<Config>
    {
        #region 对象操作
        static Config()
        {
            // 累加字段
            //Meta.Factory.AdditionalFields.Add(__.Logins);

            // 过滤器 UserModule、TimeModule、IPModule
        }

        /// <summary>验证数据，通过抛出异常的方式提示验证失败。</summary>
        /// <param name="isNew">是否插入</param>
        public override void Valid(Boolean isNew)
        {
            // 如果没有脏数据，则不需要进行任何处理
            if (!HasDirty) return;

            // 在新插入数据或者修改了指定字段时进行修正
        }

        /// <summary>首次连接数据库时初始化数据，仅用于实体类重载，用户不应该调用该方法</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void InitData()
        {
            // InitData一般用于当数据表没有数据时添加一些默认数据，该实体类的任何第一次数据库操作都会触发该方法，默认异步调用
            if (Meta.Count > 0) return;

            if (XTrace.Debug) XTrace.WriteLine("开始初始化Config[系统配置]数据……");

            var entity = new Config();
            entity.SiteName = "COMCMS";
            entity.SiteUrl = "http://www.comcms.com";
            entity.SiteLogo = "";
            entity.ICP = "";
            entity.SiteEmail = "";
            entity.SiteTel = "";
            entity.IsCloseSite = false;
            entity.IsRewrite = 0;
            entity.LastCacheTime = DateTime.Now;
            entity.LastUpdateTime = DateTime.Now;
            entity.Insert();

            if (XTrace.Debug) XTrace.WriteLine("完成初始化Config[系统配置]数据！");
        }

        ///// <summary>已重载。基类先调用Valid(true)验证数据，然后在事务保护内调用OnInsert</summary>
        ///// <returns></returns>
        //public override Int32 Insert()
        //{
        //    return base.Insert();
        //}

        ///// <summary>已重载。在事务保护范围内处理业务，位于Valid之后</summary>
        ///// <returns></returns>
        //protected override Int32 OnDelete()
        //{
        //    return base.OnDelete();
        //}
        #endregion

        #region 扩展属性
        private AttachConfigEntity _AttachConfigEntity;
        /// <summary>
        /// 附件设置实体
        /// </summary>
        public AttachConfigEntity AttachConfigEntity
        {
            get
            {
                if (_AttachConfigEntity == null && !Dirtys["AttachConfig"])
                {
                    OtherConfig ac = OtherConfig.Find(OtherConfig._.ConfigName == "attach");

                    if (ac != null)
                    {
                        if (!string.IsNullOrEmpty(ac.ConfigValue))
                        {
                            _AttachConfigEntity = JsonConvert.DeserializeObject<AttachConfigEntity>(ac.ConfigValue);
                            Dirtys["AttachConfig"] = true;
                        }
                        else
                        {
                            _AttachConfigEntity = new AttachConfigEntity();
                        }
                    }
                    else
                    {
                        _AttachConfigEntity = new AttachConfigEntity();
                        OtherConfig newac = new OtherConfig();
                        newac.ConfigName = "attach";
                        newac.ConfigValue = JsonConvert.SerializeObject(_AttachConfigEntity);
                        newac.LastUpdateTime = DateTime.Now;
                        newac.Insert();
                    }

                }
                return _AttachConfigEntity;
            }
            set { _AttachConfigEntity = value; }
        }


        private SMTPConfigEntity _SMTPConfigEntity;
        /// <summary>
        /// SMTP配置实体
        /// </summary>
        public SMTPConfigEntity SMTPConfigEntity
        {
            get
            {
                if (_SMTPConfigEntity == null && !Dirtys["SMTPConfig"])
                {
                    OtherConfig ac = OtherConfig.Find(OtherConfig._.ConfigName == "smtp");

                    if (ac != null)
                    {
                        _SMTPConfigEntity = JsonConvert.DeserializeObject<SMTPConfigEntity>(ac.ConfigValue);
                        Dirtys["SMTPConfig"] = true;
                    }
                    else
                    {
                        _SMTPConfigEntity = new SMTPConfigEntity();
                        OtherConfig newac = new OtherConfig();
                        newac.ConfigName = "smtp";
                        newac.ConfigValue = JsonConvert.SerializeObject(_SMTPConfigEntity);
                        newac.LastUpdateTime = DateTime.Now;
                        newac.Insert();
                    }
                }
                return _SMTPConfigEntity;
            }
            set { _SMTPConfigEntity = value; }
        }


        #endregion

        #region 扩展查询
        /// <summary>根据Id查找</summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static Config FindById(Int32 id)
        {
            if (Meta.Count >= 1000)
                return Find(__.Id, id);
            else // 实体缓存
                return Meta.Cache.Entities.FirstOrDefault(x => x.Id == id);
            // 单对象缓存
            //return Meta.SingleCache[id];
        }
        #endregion

        #region 高级查询
        #endregion

        #region 业务操作
        /// <summary>
        /// 获取系统配置
        /// </summary>
        /// <returns></returns>
        public static Config GetSystemConfig()
        {
            Config cfg = FindById(1);
            if (cfg == null)
            {
                cfg = Find(Config._.Id == 1);
            }
            return cfg;
        }
        #endregion
    }
}