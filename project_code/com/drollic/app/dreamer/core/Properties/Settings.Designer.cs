﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace com.drollic.app.dreamer.core.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www.drollic.com/projects/dreamer/webservices/DreamerWebServices.php")]
        public string com_drollic_app_dreamer_core_com_drollic_app_dreamer_webservices_DreamerWebService {
            get {
                return ((string)(this["com_drollic_app_dreamer_core_com_drollic_app_dreamer_webservices_DreamerWebServic" +
                    "e"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www.drollic.com/projects/dreamer/webservices/DreamerBlogWebServices.php")]
        public string com_drollic_app_dreamer_core_com_drollic_www_DreamerBlogWebService {
            get {
                return ((string)(this["com_drollic_app_dreamer_core_com_drollic_www_DreamerBlogWebService"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www.drollic.com/projects/dreamer/webservices/DreamerOfflineServices.php")]
        public string com_drollic_app_dreamer_core_com_drollic_www_dreamer_webservices_offline_DreamerOfflineService {
            get {
                return ((string)(this["com_drollic_app_dreamer_core_com_drollic_www_dreamer_webservices_offline_DreamerO" +
                    "fflineService"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www.drollic.com/projects/dreamer/webservices/DreamerOfflineVideoServices.p" +
            "hp")]
        public string com_drollic_app_dreamer_core_com_drollic_app_dreamer_webservices_offlinevideo_DreamerOfflineVideoService {
            get {
                return ((string)(this["com_drollic_app_dreamer_core_com_drollic_app_dreamer_webservices_offlinevideo_Dre" +
                    "amerOfflineVideoService"]));
            }
        }
    }
}
