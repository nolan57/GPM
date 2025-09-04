using GPM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = GPM.Models.Task;

namespace GPM.Services
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            using (var context = new ApplicationDbContext())
            {
                // 创建数据库（如果不存在）
                context.Database.EnsureCreated();

                // 如果已经有项目数据，就不再添加种子数据
                if (context.Projects.Any())
                {
                    return;
                }

                // 添加种子数据
                var projects = new List<Project>
                {
                    new Project
                    {
                        Name = "项目计划管理系统开发",
                        Description = "开发一个基于WPF的项目计划管理系统，包含甘特图功能",
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(90),
                        Tasks = new List<Task>
                        {
                            new Task
                            {
                                Name = "需求分析",
                                Description = "收集和分析用户需求",
                                StartDate = DateTime.Now,
                                EndDate = DateTime.Now.AddDays(14),
                                Progress = 0.0,
                                SubTasks = new List<SubTask>
                                {
                                    new SubTask
                                    {
                                        Name = "用户访谈",
                                        Description = "与潜在用户进行访谈",
                                        StartDate = DateTime.Now,
                                        EndDate = DateTime.Now.AddDays(7),
                                        Progress = 0.0
                                    },
                                    new SubTask
                                    {
                                        Name = "需求文档编写",
                                        Description = "编写详细的需求文档",
                                        StartDate = DateTime.Now.AddDays(7),
                                        EndDate = DateTime.Now.AddDays(14),
                                        Progress = 0.0
                                    }
                                }
                            },
                            new Task
                            {
                                Name = "系统设计",
                                Description = "设计系统架构和数据库",
                                StartDate = DateTime.Now.AddDays(14),
                                EndDate = DateTime.Now.AddDays(28),
                                Progress = 0.0
                            }
                        }
                    },
                    new Project
                    {
                        Name = "市场推广活动",
                        Description = "为新产品策划和执行市场推广活动",
                        StartDate = DateTime.Now.AddDays(30),
                        EndDate = DateTime.Now.AddDays(60),
                        Tasks = new List<Task>
                        {
                            new Task
                            {
                                Name = "市场调研",
                                Description = "分析目标市场和竞争对手",
                                StartDate = DateTime.Now.AddDays(30),
                                EndDate = DateTime.Now.AddDays(40),
                                Progress = 0.0
                            },
                            new Task
                            {
                                Name = "推广材料准备",
                                Description = "设计和制作推广材料",
                                StartDate = DateTime.Now.AddDays(40),
                                EndDate = DateTime.Now.AddDays(50),
                                Progress = 0.0
                            },
                            new Task
                            {
                                Name = "活动执行",
                                Description = "执行市场推广活动",
                                StartDate = DateTime.Now.AddDays(50),
                                EndDate = DateTime.Now.AddDays(60),
                                Progress = 0.0
                            }
                        }
                    }
                };

                // 添加项目到数据库
                context.Projects.AddRange(projects);
                context.SaveChanges();

                // 添加一些TaskModel数据
                var taskModels = new List<TaskModel>
                {
                    new TaskModel
                    {
                        Name = "项目启动会议",
                        Description = "召开项目启动会议，明确项目目标和分工",
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(1),
                        Progress = 0.0
                    },
                    new TaskModel
                    {
                        Name = "技术选型",
                        Description = "选择适合项目的技术栈",
                        StartDate = DateTime.Now.AddDays(1),
                        EndDate = DateTime.Now.AddDays(3),
                        Progress = 0.0
                    }
                };

                // 添加TaskModel到数据库
                context.TaskModels.AddRange(taskModels);
                context.SaveChanges();
            }
        }
    }
}