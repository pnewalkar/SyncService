node{
   stage('SCM Checkout'){
     git 'https://github.com/pnewalkar/SyncService'
   }
   stage('Dotnet Restore'){
   // def mvnHome =  tool name: 'maven_3_5_0', type: 'maven'   
        sh "dotnet restore --configfile NuGet.Config"
   }
    stage('DOtnet Build'){
   //def mvnHome =  tool name: 'maven_3_5_0', type: 'maven'   
        sh 'dotnet build --configuration Release'
   }
}
