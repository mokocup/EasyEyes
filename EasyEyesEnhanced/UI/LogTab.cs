using EasyEyesEnhanced.Structs.Vfx;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.UI;
using static EasyEyesEnhanced.Plugin;

namespace EasyEyesEnhanced.UI {
    public class LogTab {
        public Plugin Plugin;

        public LogTab(Plugin plugin ) {
            Plugin = plugin;
        }

        public string SelectedLogPath = "";
        
        public string FilterVFXPath = "";

        public void Draw() {
            var displayItems = Plugin.Recorded.FindAll( x=>x.path.Contains( FilterVFXPath ) );

            if( !displayItems.Exists( x => x.path.Equals( SelectedLogPath ) ) )
            {
                SelectedLogPath = "";
            }
            var ret = ImGui.BeginTabItem( "Recent VFXs##MainInterfaceTabs" );
            if( !ret ) return;

            var Id = "##Log";
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            if( Plugin.DoRecord ) {
                if(MainInterface.RemoveButton("Stop" + Id ) ) {
                    Plugin.DoRecord = false;
                }
            }
            else {
                if(MainInterface.OkButton("Record" + Id ) ) {
                    Plugin.DoRecord = true;
                }
            }
            ImGui.SameLine();
            
            if( ImGui.Button( "Reset" + Id ) ) {
                Plugin.ClearRecord();
                SelectedLogPath = "";
            }
            
            var selectDisabled = string.IsNullOrEmpty( SelectedLogPath );
            var multiDisabled = Plugin.Recorded.Count == 0;
                

            // ========= ADD ========
            if( selectDisabled ) ImGui.PushStyleVar( ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f );
            
            ImGui.SameLine();
            
            if( ImGui.Button( "Add To Blacklist" + Id ) && !selectDisabled ) {
                Plugin.Config.AddPath( SelectedLogPath, out var newItem );
            }
            
            if( selectDisabled ) ImGui.PopStyleVar();

            if( multiDisabled ) ImGui.PushStyleVar( ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f );

            ImGui.SameLine();
            if( ImGui.Button( "Add All To Blacklist" + Id ) && !multiDisabled )
            {
                Plugin.Config.AddPath( displayItems.Select( x => x.path ).ToList() );
                Plugin.ClearRecord();
                SelectedLogPath = "";
            }
            
            if( multiDisabled ) ImGui.PopStyleVar();
            
            // ======== SPAWN / REMOVE =========
            Plugin.MainUI.DrawSpawnButton( "Spawn", Id, SelectedLogPath, selectDisabled );

            // ======== Filter input =========
            ImGui.InputText( Id + "Filter", ref FilterVFXPath, 255 );
            
            //=======================
            ImGui.BeginChild( Id + "Tree", new Vector2(-1, -1), true );

            if( displayItems.Count > 0 ) {
                VFXSelectDialog.DisplayVisible( displayItems.Count, out var preItems, out var showItems, out var postItems, out var itemHeight );
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + preItems * itemHeight );
                var idx = 0;
                foreach( var item in displayItems ) {
                    if( idx < preItems || idx > ( preItems + showItems ) ) { idx++; continue; }
                    if( ImGui.Selectable( item.path + Id + idx, SelectedLogPath == item.path ) ) {
                        SelectedLogPath = item.path;
                    }
                    idx++;
                }
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + postItems * itemHeight );
            }
            else {
                ImGui.Text( "Press [Record] to view recent VFXs..." );
            }
            ImGui.EndChild();
            ImGui.EndTabItem();
        }
    }
}
