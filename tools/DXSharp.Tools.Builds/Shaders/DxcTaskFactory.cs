using System ;
using System.Linq ;
using System.Collections.Generic ;
using Microsoft.Build.Framework ;
using Microsoft.Build.Utilities ;
using MSBuildTask = Microsoft.Build.Utilities.Task ;

namespace DXSharp.Tools.Builds.Shaders {
	
	public class DxcTaskFactory: ITaskFactory2 {
		IDictionary< string, TaskPropertyInfo > parameterGroup ;
		IBuildEngine                            buildEngine ;
		string                                  taskBody ;
		
		public string FactoryName => nameof( DxcTaskFactory ) ;
		public Type TaskType => typeof( DxcTool ) ;

		public bool Initialize( string taskName, IDictionary< string, TaskPropertyInfo > _parameterGroup, 
							    string _taskBody, IBuildEngine taskFactoryLoggingHost ) {
			this.parameterGroup = _parameterGroup;
			this.taskBody       = _taskBody;
			this.buildEngine    = taskFactoryLoggingHost;

			return true;
		}
		
		public TaskPropertyInfo[ ] GetTaskParameters( ) {
			return parameterGroup.Values.ToArray( ) ;
		}
		
		public ITask CreateTask( IBuildEngine taskFactoryLoggingHost ) {
			return new DxcTool( ) ;
		}
		
		public void CleanupTask( ITask task ) {
			// Custom cleanup logic goes here
		}
		
		public bool Initialize( string taskName, IDictionary< string, string > factoryIdentityParameters, 
								IDictionary< string, TaskPropertyInfo > _parameterGroup, string _taskBody, 
								IBuildEngine taskFactoryLoggingHost ) {
			this.parameterGroup = _parameterGroup ;
			this.taskBody       = _taskBody ;
			this.buildEngine    = taskFactoryLoggingHost ;

			return true;
		}
		
		public ITask CreateTask( IBuildEngine taskFactoryLoggingHost, 
								 IDictionary< string, string > taskIdentityParameters) {
			return new DxcTool( ) ;
		}
	}
}
