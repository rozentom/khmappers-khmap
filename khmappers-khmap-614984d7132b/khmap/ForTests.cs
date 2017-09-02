using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using khmap.Controllers;
namespace khmap
{
    public class ForTests
    {
        public static void addNewFolder(string parentID, string folderName, string folderDescription)
        {
            MapFolderController mfc = new MapFolderController();
            mfc.addNewFolder(parentID, folderName, folderDescription);
        }
        public static void deleteFolder(string currFolder)
        {
            MapFolderController mfc = new MapFolderController();
            mfc.deleteFolder(currFolder);
        }
        public static void moveFolder(string folderToMoveId, string moveToFolderId)
        {
            MapFolderController mfc = new MapFolderController();
            mfc.MoveFolderToFolder(folderToMoveId, moveToFolderId);

        }
        public static void moveMap(string mapToMoveId, string moveToFolderId)
        {
            MapFolderController mfc = new MapFolderController();
            mfc.MoveMapToFolder(mapToMoveId, moveToFolderId);
        }
    }
}