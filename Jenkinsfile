node{
   stage('SCM Checkout'){
     git 'https://github.com/pnewalkar/SyncService'
   }
   stage('Dotnet Build'){
   // def mvnHome =  tool name: 'maven_3_5_0', type: 'maven'   
      sh "dotnet build"
   }
     // stage('Deploy'){
      //def mvnHome =  tool name: 'maven_3_5_0', type: 'maven'   
      //sh "${mvnHome}/bin/mvn deploy"
   //}
}
