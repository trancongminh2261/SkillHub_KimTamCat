using LMS_Project.Areas.Models;
using LMS_Project.Areas.Request;
using LMS_Project.LMS;
using LMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using static LMS_Project.Models.lmsEnum;

namespace LMS_Project.Services
{

    public class ExamSectionService
    {
        public static async Task<tbl_ExamSection> Insert(ExamSectionCreate examSectionCreate, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var exam = await db.tbl_Exam.SingleOrDefaultAsync(x => x.Id == examSectionCreate.ExamId);
                    if (exam == null)
                        throw new Exception("Không tìm thấy đề");
                    int index = 1;
                    var lastSection = await db.tbl_ExamSection
                        .Where(x => x.ExamId == examSectionCreate.ExamId && x.Enable == true).OrderByDescending(x => x.Index).FirstOrDefaultAsync();
                    if (lastSection != null)
                        index = lastSection.Index.Value + 1;
                    var model = new tbl_ExamSection(examSectionCreate);
                    model.ModifiedBy = model.CreatedBy = user.FullName;
                    model.Index = index;
                    db.tbl_ExamSection.Add(model);
                    await db.SaveChangesAsync();
                    return model;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task<tbl_ExamSection> Update(ExamSectionUpdate model, tbl_UserInformation user)
        {
            using (var db = new lmsDbContext())
            {
                try
                {
                    var entity = await db.tbl_ExamSection.SingleOrDefaultAsync(x => x.Id == model.Id);
                    if (entity == null)
                        throw new Exception("Không tìm thấy dữ liệu");
                    entity.Name = model.Name ?? entity.Name;
                    entity.Explanations = model.Explanations ?? entity.Explanations;
                    entity.ModifiedBy = user.FullName;
                    entity.ModifiedOn = DateTime.Now;
                    await db.SaveChangesAsync();
                    return entity;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public static async Task Delete(int id)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = await db.tbl_ExamSection.SingleOrDefaultAsync(x => x.Id == id);
                        if (entity == null)
                            throw new Exception("Không tìm thấy dữ liệu");
                        entity.Enable = false;
                        await db.SaveChangesAsync();
                        var groups = await db.tbl_ExerciseGroup.Where(x => x.ExamSectionId == entity.Id && x.Enable == true)
                            .Select(x=>x.Id).ToListAsync();
                        if (groups.Any())
                        {
                            foreach (var item in groups)
                            {
                                var exerciseGroup = await db.tbl_ExerciseGroup.SingleOrDefaultAsync(x => x.Id == item);
                                exerciseGroup.Enable = false;
                                await db.SaveChangesAsync();
                                var exercises = await db.tbl_Exercise.Where(x => x.ExerciseGroupId == item)
                                    .Select(x => x.Id).ToListAsync();
                                if (exercises.Any())
                                    foreach (var jtem in exercises)
                                    {
                                        var exercise = await db.tbl_Exercise.SingleOrDefaultAsync(x => x.Id == jtem);
                                        exercise.Enable = false;
                                        await db.SaveChangesAsync();
                                    }
                            }
                        }
                        await ReloadIndex(db,entity.ExamId.Value);
                        await ExerciseGroupService.UpdateExerciseNumber(db,entity.ExamId.Value);
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                }
            }
        }
        public class SectionIndexModel
        {
            public List<SectionIndexItem> Items { get; set; }
        }
        public class SectionIndexItem
        {
            public int Id { get; set; }
            public int Index { get; set; }
        }
        public static async Task ChangeSectionIndex(SectionIndexModel model)
        {
            using (var db = new lmsDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (!model.Items.Any())
                            throw new Exception("Không tìm thấy dữ liệu");
                        foreach (var item in model.Items)
                        {
                            var section = await db.tbl_ExamSection.SingleOrDefaultAsync(x => x.Id == item.Id);
                            section.Index = item.Index;
                            await db.SaveChangesAsync();
                        }
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// Cập nhật lại vị trí
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        public static async Task ReloadIndex(lmsDbContext dbContext, int examId)
        {
            var examSections = await dbContext.tbl_ExamSection.Where(x => x.ExamId == examId && x.Enable == true).OrderBy(x => x.Index).ToListAsync();
            if (examSections.Any())
            {
                int index = 1;
                foreach (var item in examSections)
                {
                    var examSection = await dbContext.tbl_ExamSection.SingleOrDefaultAsync(x => x.Id == item.Id);
                    examSection.Index = index;
                    await dbContext.SaveChangesAsync();
                    index += 1;
                }
            }
        }
    }
}