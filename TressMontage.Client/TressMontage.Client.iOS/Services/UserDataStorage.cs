using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TressMontage.Client.iOS.Services
{
    /// <summary>
    /// 
    /// IMPORTANT: Do not use this implementation directly, use the decorated "UserDataStorageLockedDecorator".
    /// 
    /// User data storage based on filesystem. 
    /// Root folder is based on user credentials.
    /// Subfolders will have name from the project they contain.
    /// 
    /// [User Hans]
    /// => [Project A]
    /// ===> Raw.json
    /// ===> [Supervision A1]
    /// =====> Raw.json
    /// =====> [Observation A1.1]
    /// =======> Attachment11
    /// =======> Attachment12
    /// =====> [Observation A1.2]
    /// =======> Attachment21
    /// =======> Attachment22
    /// => [AdHocObservations]
    /// ===> [AdHocObservation A1.1]
    /// =====> Raw.json
    /// =====> Attachment11
    /// =====> Attachment12
    /// ===> [AdHocObservation A1.2]
    /// =====> Raw.json
    /// =====> Attachment21
    /// =====> Attachment22
    /// => [ArchivedSupervision A1]
    /// ===> Raw.json
    /// ===> [ArchivedObservation A1.1]
    /// =====> Attachment11
    /// =====> Attachment12
    /// ===> [ArchivedObservation A1.2]
    /// =====> Attachment21
    /// =====> Attachment22
    /// </summary>
    public class UserDataStorage : StorageBase, IUserDataStorage
    {
        private const int MaxNumberOfDocumentFolders = 5;
        private const int MaxNumberOfUploadedSupervisions = 10;

        private const string AdHocObservationFolderName = "AdHocObservations";

        private const string DocumentsFolderName = "documents";
        private const string ProjectsBaseFolderName = "project_id_";
        private const string SupervisionBaseFolderName = "supervision_id_";
        private const string ObservationBaseFolderName = "observation_id_";
        private const string AdHocObservationBaseFolderName = "adhocobservation_id_";
        private const string ArchivedSupervisionBaseFolderName = "archivedsupervision_id_";
        private const string ArchivedObservationBaseFolderName = "archivedobservation_id_";
        private const string UserBaseFolderName = "user_id_";

        private const string RawDataFileName = "raw";
        private const string LastUploadedFileName = "LastUploaded";
        private const string SubmissionCompleteFileName = "SubmissionComplete";

        public UserDataStorage(IStringSerializer serializer, IFileSystem fileSystem)
            : base(serializer, fileSystem)
        {
        }

        /// <summary>
        /// The user intials that the component has been initialized with.
        /// </summary>
        public IUserSession Session { get; private set; }

        /// <summary>
        /// Path to User folder.
        /// </summary>
        private string UserFolderPath { get; set; }

        /// <summary>
        /// Path to AdHocObservation folder.
        /// </summary>
        private string AdHocObservationsFolderPath { get; set; }

        /// <summary>
        /// Initialize component to work in the context of the given user.
        /// </summary>
        public async Task InitializeAsync(IUserSession userSession)
        {
            ThrowIfInitializedBefore();

            userSession.ThrowIfParameterIsNull(nameof(userSession));
            userSession.User.Initials.ThrowIfParameterIsNullOrWhiteSpace(nameof(userSession.User.Initials));

            Session = userSession;
            await CreateFolderStructureAsync();
            IsInitialized = true;
        }

        /// <summary>
        /// Close the component so that it is no longer initialized.
        /// To use it again it must be Initialized.
        /// </summary>
        public void Close()
        {
            if (IsInitialized)
            {
                Session = null;
                IsInitialized = false;
            }
        }

        /// <summary>
        /// Delete all data for user and Close the component.
        /// </summary>
        public async Task ClearAsync()
        {
            ThrowIfNotInitialized();

            await FileSystem.DeleteFolderIfExistsAsync(UserFolderPath);
            Close();
        }

        #region Project

        /// <summary>
        /// Get local project by id if it exists; otherwise returns 'null'.
        /// </summary>
        public async Task<ProjectLocal> GetProjectByIdAsync(string projectId)
        {
            ThrowIfNotInitialized();

            var projectFolderPath = BuildProjectFolderPath(projectId);
            var dataFilePath = BuildDataFormatFileName(Path.Combine(projectFolderPath, RawDataFileName));

            return await ReadObjectFromFileOrDefaultAsync<ProjectLocal>(dataFilePath);
        }

        /// <summary>
        /// Get lists of local projects (can be empty but will be instantiated).
        /// </summary>
        public async Task<IList<ProjectLocal>> GetProjectsAsync()
        {
            ThrowIfNotInitialized();

            var projects = new List<ProjectLocal>();

            var projectsSearchPattern = BuildStartsWithSearchPattern(ProjectsBaseFolderName);
            var projectFolders = await FileSystem.SearchFolderPathsInFolderAsync(UserFolderPath, projectsSearchPattern);
            foreach (var projectFolder in projectFolders)
            {
                var dataFilePath = BuildDataFormatFileName(Path.Combine(projectFolder, RawDataFileName));

                var projectLocal = await ReadObjectFromFileOrDefaultAsync<ProjectLocal>(dataFilePath);
                if (projectLocal != null)
                {
                    projects.Add(projectLocal);
                }
            }

            return projects;
        }

        /// <summary>
        /// Insert or update project primary data and update "last changed timestamp for document folder".
        /// </summary>
        public async Task InsertOrUpdateProjectPrimaryDataAndTimestampAsync(ProjectDto projectDto)
        {
            ThrowIfNotInitialized();

            var projectLocal = await CreateOrUpdateProjectLocalAsync(projectDto);
            projectLocal.LastChangedTimestampForDocumentFolder = DateTime.UtcNow;
            await InsertOrUpdateProjectLocalAsync(projectLocal);

            await CleanProjectDocumentFolderAsync();
        }

        /// <summary>
        /// Insert or update projects primary data (does not change "last changed timestamp for document folder").
        /// </summary>
        public async Task InsertOrUpdateProjectsPrimaryDataAsync(IList<ProjectDto> projectDtoList)
        {
            ThrowIfNotInitialized();

            foreach (var projectDto in projectDtoList)
            {
                var projectLocal = await CreateOrUpdateProjectLocalAsync(projectDto);
                await InsertOrUpdateProjectLocalAsync(projectLocal);
            }
        }

        #endregion

        #region Supervision

        /// <summary>
        /// Get the next Supervision that should be submitted if one exists (otherwise "null");
        /// Either it will be one with state "SubmissionStarted" (e.g. we did not completely upload it)
        /// or it will be the next one that is "ReadyForSubmission".
        /// </summary>
        public async Task<SupervisionLocal> GetNextSupervisionForSynchronizationIfExistsAsync()
        {
            ThrowIfNotInitialized();

            var supervisionsTuple = await GetSupervisionsAsync();
            var submittedSupervisions = supervisionsTuple.Item2;

            var startedSupervision = submittedSupervisions
                .Where(x => x.State == SupervisionState.SubmissionStarted)
                .OrderBy(x => x.DateAddedForSubmission)
                .FirstOrDefault();

            if (startedSupervision != null)
            {
                return startedSupervision;
            }

            var readySupervision = submittedSupervisions
                .Where(x => x.State == SupervisionState.ReadyForSubmission)
                .OrderBy(x => x.DateAddedForSubmission)
                .FirstOrDefault();

            return readySupervision;
        }

        /// <summary>
        /// Get list of open supervisions (Tuple.Item1), and submitted supervisions (Tuple.Item2) together with the last uploaded supervisions.
        /// The lists can be empty but will be instantiated.
        /// </summary>
        public async Task<Tuple<IList<SupervisionLocal>, IList<SupervisionLocal>>> GetSupervisionsAsync()
        {
            ThrowIfNotInitialized();

            var availableSupervisions = await GetAvailableSupervisionsAsync();

            var openSupervisions = availableSupervisions
                .Where(x => x.State == SupervisionState.Edit)
                .ToList();

            var submittedSupervisions = availableSupervisions
                .Except(openSupervisions)
                .ToList();

            var lastUploadedSupervisionsData = await GetLastUploadedSupervisionsAsync();

            // BEWARE: We are combining submitted supervisions with the last uploaded supervisions.
            submittedSupervisions.AddRange(lastUploadedSupervisionsData);

            return new Tuple<IList<SupervisionLocal>, IList<SupervisionLocal>>(openSupervisions, submittedSupervisions);
        }

        /// <summary>
        /// Get open supervision by id if it exists; otherwise returns 'null'.
        /// </summary>
        public Task<SupervisionLocal> GetOpenSupervisionByIdAsync(Guid supervisionId)
        {
            ThrowIfNotInitialized();

            return GetObjectByIdFolderNameOrDefaultAsync<SupervisionLocal>(BuildSupervisionFolderName(supervisionId));
        }

        /// <summary>
        /// Create supervision in project, if the project exists; otherwise nothing happens.
        /// </summary>
        public async Task<SupervisionDto> CreateSupervisionAsync(string projectId)
        {
            ThrowIfNotInitialized();

            var projectData = await GetProjectByIdAsync(projectId);
            if (projectData != null)
            {
                var supervisionLocal = CreateSupervision(Session, projectData);
                await InsertOrUpdateSupervision(supervisionLocal);

                return supervisionLocal.SupervisionDto;
            }

            return null;
        }

        public Task DeleteSupervisionByIdAsync(Guid supervisionId)
        {
            ThrowIfNotInitialized();

            return DeleteObjectByIdFolderNameIsExistsAsync(BuildSupervisionFolderName(supervisionId));
        }

        /// <summary>
        /// Updates a supervision with new values/states that might have changed.
        /// </summary>
        public async Task UpdateSupervisionAsync(SupervisionLocal supervision)
        {
            ThrowIfNotInitialized();

            if (supervision != null)
            {
                await InsertOrUpdateSupervision(supervision);
            }
        }

        public async Task<SupervisionLocal> UpdateSupervisionSubmissionStateAsync(Guid supervisionId, SupervisionState state)
        {
            ThrowIfNotInitialized();

            var supervisionLocal = await GetOpenSupervisionByIdAsync(supervisionId);
            if (supervisionLocal != null)
            {
                supervisionLocal.State = state;
                await InsertOrUpdateSupervision(supervisionLocal);
            }

            return supervisionLocal;
        }

        /// <summary>
        /// If supervision with id exists and state is "submission completed", then deletes the supervision and adds it to "last uploaded" list;
        /// otherwise nothing happens.
        /// </summary>        
        public async Task MoveSupervisionToLastUploadedSupervisionsAsync(Guid supervisionId)
        {
            ThrowIfNotInitialized();

            var supervision = await GetOpenSupervisionByIdAsync(supervisionId);
            if (supervision != null && supervision.State == SupervisionState.SubmissionCompleted)
            {
                await DeleteSupervisionByIdAsync(supervision.SupervisionDto.IdGeneratedOnApp);
                await CreateOrUpdateLastUploadedSupervisionsAsync(supervision);
            }
        }

        #endregion

        #region Observation

        public async Task<ObservationDto> CreateObservationAsync(Guid supervisionId)
        {
            ThrowIfNotInitialized();

            var supervisionData = await GetOpenSupervisionByIdAsync(supervisionId);
            if (supervisionData != null)
            {
                var observation = CreateObservation();
                supervisionData.SupervisionDto.Observations.Add(observation);

                await InsertOrUpdateSupervision(supervisionData);
                return observation;
            }

            return null;
        }

        public async Task UpdateObservationValuesAsync(Guid supervisionId, Guid observationId, string name, string value)
        {
            ThrowIfNotInitialized();

            var supervision = await GetOpenSupervisionByIdAsync(supervisionId);
            if (supervision != null)
            {
                var observation = supervision.SupervisionDto.Observations.FirstOrDefault(x => x.IdGeneratedOnApp == observationId);
                if (observation != null)
                {
                    var extendedObservationModelValue = observation.ExtendedObservationModelValues.FirstOrDefault(x => x.Key == name);
                    if (extendedObservationModelValue == null)
                    {
                        observation.ExtendedObservationModelValues.Add(new ExtendedObjectModelValueDto()
                        {
                            Key = name,
                            Value = value,
                        });
                    }
                    else
                    {
                        extendedObservationModelValue.Value = value;
                    }

                    await InsertOrUpdateSupervision(supervision);
                }
            }
        }

        /// <summary>
        /// Updates the observations extended observation model with the new values in the list.
        /// Because the method can be called after we have deleted the observation,
        /// we ensure that the observation do exist. 
        /// </summary>
        public async Task UpdateObservationValuesAsync(Guid supervisionId, Guid observationId, IList<ExtendedObjectModelValueDto> extendedObservationModelValues)
        {
            ThrowIfNotInitialized();

            var supervision = await GetOpenSupervisionByIdAsync(supervisionId);
            if (supervision != null)
            {
                var observation = supervision.SupervisionDto.Observations.FirstOrDefault(x => x.IdGeneratedOnApp == observationId);
                if (observation != null)
                {
                    observation.ExtendedObservationModelValues = extendedObservationModelValues;

                    await InsertOrUpdateSupervision(supervision);
                }
            }
        }

        public async Task DeleteObservationAsync(Guid supervisionId, Guid observationId)
        {
            ThrowIfNotInitialized();

            var supervisionData = await GetOpenSupervisionByIdAsync(supervisionId);
            if (supervisionData != null)
            {
                supervisionData.SupervisionDto.Observations = supervisionData.SupervisionDto.Observations
                    .Where(x => x.IdGeneratedOnApp != observationId)
                    .ToList();

                await InsertOrUpdateSupervision(supervisionData);
            }
        }

        /// <summary>
        /// Updates the attachment file by deleting the original file with the given "attachment id" and creating a new attachment
        /// belonging to observation with the given "observation id",
        /// belonging to supervision with the given "supervision id".
        /// </summary>
        public async Task UpdateObservationAttachmentAsync(Guid supervisionId, Guid observationId, Guid attachmentId, byte[] attachmentContent)
        {
            ThrowIfNotInitialized();

            var supervision = await GetOpenSupervisionByIdAsync(supervisionId);
            if (supervision != null)
            {
                var observation = supervision.SupervisionDto.Observations.FirstOrDefault(x => x.IdGeneratedOnApp == observationId);
                if (observation != null && observation.Attachments.Any(x => x.IdGeneratedOnApp == attachmentId))
                {
                    string attachmentPath = await BuildAttachmentPath(supervisionId, observationId, attachmentId);
                    await FileSystem.DeleteFileIfExistsAsync(attachmentPath);
                    await FileSystem.WriteBytesFileAsync(attachmentPath, attachmentContent);
                }
            }
        }

        /// <summary>
        /// Create and returns attachment if possible; otherwise returns "null".
        /// </summary>
        public async Task<AttachmentLocal> CreateObservationAttachmentAsync(Guid supervisionId, Guid observationId, byte[] attachmentContent)
        {
            ThrowIfNotInitialized();

            var supervision = await GetOpenSupervisionByIdAsync(supervisionId);
            if (supervision != null)
            {
                var observation = supervision.SupervisionDto.Observations.FirstOrDefault(x => x.IdGeneratedOnApp == observationId);
                if (observation != null)
                {
                    var attachment = CreateAttachment(attachmentContent);
                    var attachmentPath = await BuildAttachmentPath(supervisionId, observationId, attachment.AttachmentDto.IdGeneratedOnApp);
                    await FileSystem.WriteBytesFileAsync(attachmentPath, attachment.Content);

                    observation.Attachments.Add(attachment.AttachmentDto);
                    await InsertOrUpdateSupervision(supervision);

                    return attachment;
                }
            }

            return null;
        }

        public async Task DeleteObservationAttachmentAsync(Guid supervisionId, Guid observationId, Guid attachmentId)
        {
            ThrowIfNotInitialized();

            var supervision = await GetOpenSupervisionByIdAsync(supervisionId);
            if (supervision != null)
            {
                var observation = supervision.SupervisionDto.Observations.FirstOrDefault(x => x.IdGeneratedOnApp == observationId);
                if (observation != null)
                {
                    string attachmentPath = await BuildAttachmentPath(supervisionId, observationId, attachmentId);
                    await FileSystem.DeleteFileIfExistsAsync(attachmentPath);

                    observation.Attachments = observation.Attachments
                        .Where(x => x.IdGeneratedOnApp != attachmentId)
                        .ToList();

                    await InsertOrUpdateSupervision(supervision);
                }
            }
        }

        #endregion

        #region AdHocObservation

        public async Task<AdHocObservationLocal> CreateAdHocObservationAsync()
        {
            ThrowIfNotInitialized();

            var newAdHocObservation = CreateAdHocObservation();

            var newAdHocObservationFolderName = BuildAdHocObservationFolderName(newAdHocObservation.IdGeneratedOnApp);
            var newAdHocObservationFolderPath = await CreateFolderIfNotExistsAsync(AdHocObservationsFolderPath, newAdHocObservationFolderName);

            await WriteObjectByIdFolderNameAsync(newAdHocObservationFolderPath, newAdHocObservation);

            return newAdHocObservation;
        }

        public async Task<IList<AdHocObservationLocal>> GetAdHocObservationsAsync()
        {
            ThrowIfNotInitialized();

            var existingAdHocObservations = new List<AdHocObservationLocal>();

            var existingAdHocObservationSearchPattern = BuildStartsWithSearchPattern(AdHocObservationBaseFolderName);
            var rawDataFileName = BuildDataFormatFileName(RawDataFileName);

            var existingAdHocObservationFolders = await FileSystem.SearchFolderPathsInFolderAsync(AdHocObservationsFolderPath, existingAdHocObservationSearchPattern);
            foreach (var existingAdHocObservationFolder in existingAdHocObservationFolders)
            {
                var fullAdHocObservationFilename = Path.Combine(existingAdHocObservationFolder, rawDataFileName);

                var existingAdHocObservation = await ReadObjectFromFileOrDefaultAsync<AdHocObservationLocal>(fullAdHocObservationFilename);
                if (existingAdHocObservation != null)
                {
                    existingAdHocObservations.Add(existingAdHocObservation);
                }
            }

            return existingAdHocObservations;
        }

        /// <summary>
        /// Get AdHoc observation by id if it exists; otherwise returns 'null'.
        /// </summary>
        public Task<AdHocObservationLocal> GetAdHocObservationByIdAsync(Guid existingAdHocObservationId)
        {
            ThrowIfNotInitialized();

            var existingAdHocObservationFolderName = BuildAdHocObservationFolderName(existingAdHocObservationId);
            var existingAdHocObservationFolderPath = Path.Combine(AdHocObservationsFolderPath, existingAdHocObservationFolderName);
                                                     
            return GetObjectByIdFolderNameOrDefaultAsync<AdHocObservationLocal>(existingAdHocObservationFolderPath);
        }
        
        public async Task UpdateAdHocObservationValuesAsync(Guid updatedAdHocObservationId, string note)
        {
            ThrowIfNotInitialized();

            var existingAdHocObservationFolderName = BuildAdHocObservationFolderName(updatedAdHocObservationId);
            var existingAdHocObservationFolderPath = Path.Combine(AdHocObservationsFolderPath, existingAdHocObservationFolderName);

            var existingAdHocObservation = await GetObjectByIdFolderNameOrDefaultAsync<AdHocObservationLocal>(existingAdHocObservationFolderPath);
            if (existingAdHocObservation != null)
            {
                existingAdHocObservation.Note = note;

                await WriteObjectByIdFolderNameAsync(existingAdHocObservationFolderPath, existingAdHocObservation);
            }
        }

        public Task DeleteAdHocObservationByIdAsync(Guid existingAdHocObservationId)
        {
            ThrowIfNotInitialized();

            var existingAdHocObservationFolderName = BuildAdHocObservationFolderName(existingAdHocObservationId);
            return DeleteObjectByIdFolderNameIsExistsAsync(Path.Combine(AdHocObservationsFolderPath, existingAdHocObservationFolderName));
        }

        /// <summary>
        /// Updates the attachment file by deleting the original file with the given "attachment id" and creating a new attachment
        /// belonging to observation with the given "observation id".
        /// </summary>
        public async Task UpdateAdHocObservationAttachmentAsync(Guid existingAdHocObservationId, Guid attachmentId, byte[] attachmentContent)
        {
            ThrowIfNotInitialized();

            var existingAdHocObservationFolderName = BuildAdHocObservationFolderName(existingAdHocObservationId);
            var existingAdHocObservationFolderPath = Path.Combine(AdHocObservationsFolderPath, existingAdHocObservationFolderName);

            var existingAdHocObservation = await GetObjectByIdFolderNameOrDefaultAsync<AdHocObservationLocal>(existingAdHocObservationFolderPath);
            if (existingAdHocObservation != null && existingAdHocObservation.Attachments.Any(x => x.IdGeneratedOnApp == attachmentId))
            {
                var attachmentPath = BuildAdHocAttachmentPath(existingAdHocObservationFolderPath, attachmentId);
                await FileSystem.DeleteFileIfExistsAsync(attachmentPath);
                await FileSystem.WriteBytesFileAsync(attachmentPath, attachmentContent);
            }
        }

        /// <summary>
        /// Create and returns attachment if possible; otherwise returns "null".
        /// </summary>
        public async Task<AttachmentLocal> CreateAdHocObservationAttachmentAsync(Guid existingAdHocObservationId, byte[] attachmentContent)
        {
            ThrowIfNotInitialized();

            var existingAdHocObservationFolderName = BuildAdHocObservationFolderName(existingAdHocObservationId);
            var existingAdHocObservationFolderPath = Path.Combine(AdHocObservationsFolderPath, existingAdHocObservationFolderName);

            var existingAdHocObservation = await GetObjectByIdFolderNameOrDefaultAsync<AdHocObservationLocal>(existingAdHocObservationFolderPath);
            if (existingAdHocObservation != null)
            {
                var attachment = CreateAttachment(attachmentContent);
                var attachmentPath = BuildAdHocAttachmentPath(existingAdHocObservationFolderPath, attachment.AttachmentDto.IdGeneratedOnApp);
                await FileSystem.WriteBytesFileAsync(attachmentPath, attachment.Content);

                existingAdHocObservation.Attachments.Add(attachment.AttachmentDto);
                await WriteObjectByIdFolderNameAsync(existingAdHocObservationFolderPath, existingAdHocObservation);

                return attachment;
            }

            return null;
        }

        public async Task DeleteAdHocObservationAttachmentAsync(Guid existingAdHocObservationId, Guid attachmentId)
        {
            ThrowIfNotInitialized();

            var existingAdHocObservationFolderName = BuildAdHocObservationFolderName(existingAdHocObservationId);
            var existingAdHocObservationFolderPath = Path.Combine(AdHocObservationsFolderPath, existingAdHocObservationFolderName);

            var existingAdHocObservation = await GetObjectByIdFolderNameOrDefaultAsync<AdHocObservationLocal>(existingAdHocObservationFolderPath);
            if (existingAdHocObservation != null)
            {
                var attachmentPath = BuildAdHocAttachmentPath(existingAdHocObservationFolderPath, attachmentId);
                await FileSystem.DeleteFileIfExistsAsync(attachmentPath);

                existingAdHocObservation.Attachments = existingAdHocObservation.Attachments
                    .Where(x => x.IdGeneratedOnApp != attachmentId)
                    .ToList();

                await WriteObjectByIdFolderNameAsync(existingAdHocObservationFolderPath, existingAdHocObservation);
            }
        }

        #endregion

        #region Attachment

        /// <summary>
        /// Returns the attachment with the given id if possible; otherwise "null".
        /// </summary>
        public async Task<byte[]> GetAttachmentByIdAsync(Guid attachmentId)
        {
            ThrowIfNotInitialized();

            string attachmentPath = await GetAttachmentPathFromId(attachmentId);
            if (attachmentPath != null)
            {
                return await FileSystem.ReadBytesFromFileAsync(attachmentPath);
            }

            return null;
        }

        /// <summary>
        /// Returns a stream of the attachment with the given id if possible; otherwise "null".
        /// This is effecient if we wan't to work with the direct file stream of the attachment,
        /// and do not want to load it into memory.
        /// 
        /// IMPORTANT: Remember to dispose the stream after use.
        /// </summary>
        public async Task<Stream> GetAttachmentStreamByIdAsync(Guid attachmentId)
        {
            ThrowIfNotInitialized();

            string attachmentPath = await GetAttachmentPathFromId(attachmentId);
            if (attachmentPath != null)
            {
                return await FileSystem.ReadStreamFromFileAsync(attachmentPath);
            }

            return null;
        }

        #endregion

        #region Document

        /// <summary>
        /// Retrieve document info.
        /// If the file does not exist the DocumentInfo.LastWriteTime will be DateTime.MinValue.
        /// </summary>
        public async Task<DocumentInfo> GetDocumentInfoAsync(string projectId, DocumentKeyInfoDto documentDto)
        {
            ThrowIfNotInitialized();

            var documentsFolderPath = BuildDocumentsFolderPath(projectId);
            var documentFilePath = Path.Combine(documentsFolderPath, BuildDocumentFileName(documentDto));

            var lastWriteTimeUtc = await FileSystem.GetLastWriteTimeUtcAsync(documentFilePath);

            return new DocumentInfo(documentFilePath, lastWriteTimeUtc);
        }

        /// <summary>
        /// Write or overwrite the given document (stream).
        /// 
        /// IMPORTANT: Remember to dispose the stream after use.
        /// </summary>
        public async Task UpdateDocumentAsync(string projectId, DocumentKeyInfoDto documentDto, Stream stream)
        {
            ThrowIfNotInitialized();

            var documentsFolderPath = BuildDocumentsFolderPath(projectId);

            var documentFilePath = Path.Combine(documentsFolderPath, BuildDocumentFileName(documentDto));
            await FileSystem.DeleteFileIfExistsAsync(documentFilePath);

            await FileSystem.WriteStreamToFileAsync(documentFilePath, stream);
            await FileSystem.SetWriteTimeUtcAsync(documentFilePath, documentDto.ChangeDate);
        }

        /// <summary>
        /// Returns a list of existing document paths for a given project.
        /// </summary>
        public Task<string[]> GetDocumentsPathsAsync(string projectId)
        {
            ThrowIfNotInitialized();

            var documentsFolderPath = BuildDocumentsFolderPath(projectId);

            return FileSystem.GetFilesFromFolderIfExistsAsync(documentsFolderPath);
        }

        /// <summary>
        /// Deletes a document at the specified path.
        /// Does not throw any exceptions if the path/file does not exist.
        /// </summary>
        public async Task DeleteDocumentAsync(string path)
        {
            ThrowIfNotInitialized();

            await FileSystem.DeleteFileIfExistsAsync(path);
        }

        /// <summary>
        /// Precondition: We expect all documents that are on the server are downloaded already.
        /// 
        /// Deletes leftover documents (e.g. if a document has been removed from the project on the server, then we now have to delete it locally as well).
        /// </summary>
        public async Task DeleteLeftoverDocumentsAsync(ProjectDto projectDto)
        {
            ThrowIfNotInitialized();

            var allLocalProjectDocuments = await GetDocumentsPathsAsync(projectDto.Id);
            var mustKeepProjectDocumentsPaths = new List<string>();

            foreach (var document in projectDto.Documents)
            {
                var documentInfo = await GetDocumentInfoAsync(projectDto.Id, document);
                mustKeepProjectDocumentsPaths.Add(documentInfo.Path);
            }

            var documentPathsToDelete = allLocalProjectDocuments.Except(mustKeepProjectDocumentsPaths);
            foreach (var document in documentPathsToDelete)
            {
                await DeleteDocumentAsync(document);
            }
        }

        #endregion

        #region Archive

        /// <summary>
        /// Creates archived supervision folder in project folder as well as the assosiated archived observations folders.
        /// </summary>
        public async Task UpdateArchivedSupervisionAsync(ArchivedSupervisionDto supervision)
        {
            ThrowIfNotInitialized();

            var archivedSupervisionFolderPath = await BuildArchivedSupervisionFolderPath(supervision.SupervisionId);

            var supervisionLocal = new ArchivedSupervisionLocal()
            {
                ArchivedSupervisionDto = supervision,
            };

            await CreateArchivedObservationsFolders(supervisionLocal, archivedSupervisionFolderPath);

            // Write to data file
            var dataFilePath = BuildDataFormatFileName(Path.Combine(archivedSupervisionFolderPath, RawDataFileName));
            await WriteObjectToFileAsync(supervisionLocal, dataFilePath);
        }

        /// <summary>
        /// Get archived supervision with the given id; otherwise "null" if it does not exist.
        /// </summary>
        public Task<ArchivedSupervisionLocal> GetArchivedSupervisionByIdAsync(Guid supervisionId)
        {
            ThrowIfNotInitialized();

            return GetObjectByIdFolderNameOrDefaultAsync<ArchivedSupervisionLocal>(BuildArchivedSupervisionFolderName(supervisionId));
        }

        /// <summary>
        /// Get archived observation belonging to the supervision with the given id; otherwise "null" if it does not exist.
        /// </summary>
        public async Task<ArchivedObservationDto> GetArchivedObservationByIdAsync(Guid supervisionId, Guid observationId)
        {
            ThrowIfNotInitialized();

            var supervision = await GetArchivedSupervisionByIdAsync(supervisionId);
            if (supervision != null)
            {
                return supervision.ArchivedSupervisionDto.ArchivedObservations.FirstOrDefault(x => x.ObservationId == observationId);
            }

            return null;
        }

        /// <summary>
        /// Returns the attachment with the given id if possible; otherwise "null".
        /// </summary>
        public async Task<byte[]> GetArchivedAttachmentByIdAsync(Guid archivedAttachmentId)
        {
            ThrowIfNotInitialized();

            var archivedAttachmentFileSearchPattern = BuildStartsWithSearchPattern(archivedAttachmentId.ToString());
            var filePaths = await FileSystem.SearchFilePathsInFolderAsync(UserFolderPath, archivedAttachmentFileSearchPattern);
            var attachmentPath = filePaths.FirstOrDefault();
            if (attachmentPath != null)
            {
                return await FileSystem.ReadBytesFromFileAsync(attachmentPath);
            }

            return null;
        }

        /// <summary>
        /// The supervision must exist otherwise one cannot rely on the output.
        /// If e.g. the app only gets half of the attachments, then it will check localstorage vs. what 
        /// needs to be downloaded and then downloads the missing attachments. 
        /// </summary>
        /// <returns>
        /// Tuple of Observation(s) and its attachments.
        /// </returns>
        public async Task<List<Tuple<Guid, List<Guid>>>> GetObservationsDataMissingAsync(Guid supervisionId)
        {
            ThrowIfNotInitialized();

            var missingArchivedObservationsData = new List<Tuple<Guid, List<Guid>>>();

            var archivedSupervision = await GetArchivedSupervisionByIdAsync(supervisionId);
            if (archivedSupervision != null)
            {
                foreach (var archivedObservation in archivedSupervision.ArchivedSupervisionDto.ArchivedObservations)
                {
                    var missingAttachmentIds = new List<Guid>();

                    foreach (var archivedAttachment in archivedObservation.Attachments)
                    {
                        var attachment = await GetArchivedAttachmentByIdAsync(archivedAttachment.IdGeneratedOnApp);
                        if (attachment == null)
                        {
                            missingAttachmentIds.Add(archivedAttachment.IdGeneratedOnApp);
                        }
                    }

                    if (missingAttachmentIds.Any())
                    {
                        missingArchivedObservationsData.Add(Tuple.Create(archivedObservation.ObservationId, missingAttachmentIds));
                    }
                }
            }

            return missingArchivedObservationsData;
        }

        /// <summary>
        /// Write or overwrite the given image (stream).
        /// 
        /// IMPORTANT: Remember to dispose the stream after use.
        /// </summary>
        public async Task UpdateArchivedAttachmentAsync(Guid supervisionId, Guid observationId, Guid attachmentId, Stream stream)
        {
            ThrowIfNotInitialized();

            var attachmentFilePath = await BuildArchivedAttachmentPath(supervisionId, observationId, attachmentId);

            await FileSystem.WriteStreamToFileAsync(attachmentFilePath, stream);
        }

        /// <summary>
        /// Deletes any old archived supervisions that are different from the one being selected.
        /// </summary>
        public async Task DeleteOtherArchivedSupervisionFolderIfExistsAsync(Guid newSupervisionId)
        {
            ThrowIfNotInitialized();

            var archivedSupervisionFolderSearchPattern = BuildStartsWithSearchPattern(ArchivedSupervisionBaseFolderName);

            // We should only have one, but we might as well just run through the array and delete.
            var archivedSupervisions = await FileSystem.SearchFolderPathsInFolderAsync(UserFolderPath, archivedSupervisionFolderSearchPattern);
            foreach (var archivedSupervision in archivedSupervisions)
            {
                if (false == archivedSupervision.Contains(newSupervisionId.ToString()))
                {
                    await FileSystem.DeleteFolderIfExistsAsync(archivedSupervision);
                }
            }
        }

        #endregion

        private static string BuildStartsWithSearchPattern(string startsWith)
        {
            return string.Format("{0}*", startsWith);
        }

        private static string BuildAdHocAttachmentPath(string existingAdHocObservationFolderPath, Guid attachmentId)
        {
            return Path.Combine(existingAdHocObservationFolderPath, attachmentId.ToString());
        }

        private static AdHocObservationLocal CreateAdHocObservation()
        {
            return new AdHocObservationLocal()
            {
                IdGeneratedOnApp = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow,
                Note = string.Empty,
                Attachments = new List<AttachmentDto>(),
            };
        }

        private static SupervisionLocal CreateSupervision(IUserSession session, ProjectLocal projectDto)
        {
            return new SupervisionLocal()
            {
                State = SupervisionState.Edit,
                SupervisionDto = new SupervisionDto
                {
                    IdGeneratedOnApp = Guid.NewGuid(),
                    ProjectId = projectDto.ProjectKeyInfoDto.Id,
                    DateCreated = DateTime.UtcNow,
                    Observations = new List<ObservationDto>(),
                    ExtendedObservationModel = projectDto.ProjectKeyInfoDto.ExtendedObservationModel,
                    SupervisorDto = new SupervisorDto()
                    {
                        FullName = session.User.FullName,
                        Initials = session.User.Initials
                    },
                }
            };
        }

        private static ObservationDto CreateObservation()
        {
            return new ObservationDto()
            {
                IdGeneratedOnApp = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow,
                Attachments = new List<AttachmentDto>(),
                ExtendedObservationModelValues = new List<ExtendedObjectModelValueDto>()
            };
        }

        private static AttachmentLocal CreateAttachment(byte[] screenShot)
        {
            return new AttachmentLocal()
            {
                AttachmentDto = new AttachmentDto()
                {
                    IdGeneratedOnApp = Guid.NewGuid()
                },
                Content = screenShot,
            };
        }

        private async Task<string> GetAttachmentPathFromId(Guid attachmentId)
        {
            var attachmentFileSearchPattern = BuildStartsWithSearchPattern(attachmentId.ToString());
            var filePaths = await FileSystem.SearchFilePathsInFolderAsync(UserFolderPath, attachmentFileSearchPattern);
            return filePaths.FirstOrDefault();
        }

        private List<SupervisionLocal> GetTenLastestUploadedSupervisions(List<SupervisionLocal> uploadedSupervisions)
        {
            var orderedProjectLocalListDescending = uploadedSupervisions
               .OrderByDescending(x => x.SupervisionDto.DateSubmitted)
               .ToList();

            var supervisionLocalListToStore = orderedProjectLocalListDescending
                .Take(MaxNumberOfUploadedSupervisions)
                .ToList();

            return supervisionLocalListToStore;
        }

        private async Task InsertOrUpdateSupervision(SupervisionLocal supervisionLocal)
        {
            var projectFolderPath = BuildProjectFolderPath(supervisionLocal.SupervisionDto.ProjectId);

            // Creates supervision folder in project folder
            var supervisionFolderName = BuildSupervisionFolderName(supervisionLocal.SupervisionDto.IdGeneratedOnApp);
            var supervisionFolderPath = await CreateFolderIfNotExistsAsync(projectFolderPath, supervisionFolderName);

            // Write supervision data file
            var dataFilePath = BuildDataFormatFileName(Path.Combine(supervisionFolderPath, RawDataFileName));
            await WriteObjectToFileAsync(supervisionLocal, dataFilePath);
        }

        private async Task<IList<SupervisionLocal>> GetAvailableSupervisionsAsync()
        {
            var supervisions = new List<SupervisionLocal>();

            var projectsSearchPattern = BuildStartsWithSearchPattern(ProjectsBaseFolderName);
            var supervisionsSearchPattern = BuildStartsWithSearchPattern(SupervisionBaseFolderName);

            var rawDataFileName = BuildDataFormatFileName(RawDataFileName);

            var projectFolders = await FileSystem.SearchFolderPathsInFolderAsync(UserFolderPath, projectsSearchPattern);
            foreach (var projectFolder in projectFolders)
            {
                var supervisionFolders = await FileSystem.SearchFolderPathsInFolderAsync(projectFolder, supervisionsSearchPattern);
                foreach (var supervisionFolder in supervisionFolders)
                {
                    var fullSupervisionFilename = Path.Combine(supervisionFolder, rawDataFileName);
                    var supervision = await ReadObjectFromFileOrDefaultAsync<SupervisionLocal>(fullSupervisionFilename);
                    if (supervision != null)
                    {
                        supervisions.Add(supervision);
                    }
                }
            }

            return supervisions;
        }

        /// <summary>
        /// Append to the list (or create it if not exist) of supervisions that have already been uploaded and have been delete from local storage.
        /// Only key info about the uploaded supervision(s) is stored.
        /// Only the lastest 10 uploaded supervisions are stored.
        /// </summary>
        private async Task CreateOrUpdateLastUploadedSupervisionsAsync(SupervisionLocal supervision)
        {
            var existingLastUploadedSupervisions = await GetLastUploadedSupervisionsAsync();
            existingLastUploadedSupervisions.Add(supervision);

            if (existingLastUploadedSupervisions.Count > MaxNumberOfUploadedSupervisions)
            {
                existingLastUploadedSupervisions = GetTenLastestUploadedSupervisions(existingLastUploadedSupervisions.ToList());
            }

            var dataFilePath = BuildDataFormatFileName(Path.Combine(EnsureValidFolderPath(UserFolderPath), LastUploadedFileName));
            await WriteObjectToFileAsync(existingLastUploadedSupervisions, dataFilePath);
        }

        /// <summary>
        /// Reads the json file in the user folder containing a list of keyinfo for supervisions that have been uploaded.
        /// 
        /// If none exists an empty list will be returned (so a valid instance).
        /// </summary>
        private async Task<IList<SupervisionLocal>> GetLastUploadedSupervisionsAsync()
        {
            var rawDataFileName = BuildDataFormatFileName(LastUploadedFileName);
            var fullSupervisionFilename = Path.Combine(EnsureValidFolderPath(UserFolderPath), rawDataFileName);

            return await ReadObjectFromFileOrDefaultAsync<List<SupervisionLocal>>(fullSupervisionFilename);
        }

        private async Task InsertOrUpdateProjectLocalAsync(ProjectLocal projectLocal)
        {
            // Creates project folder in user folder
            var projectFolderName = BuildProjectFolderName(projectLocal.ProjectKeyInfoDto.Id);
            var projectFolderPath = await CreateFolderIfNotExistsAsync(UserFolderPath, projectFolderName);

            var dataFilePath = BuildDataFormatFileName(Path.Combine(projectFolderPath, RawDataFileName));
            await WriteObjectToFileAsync(projectLocal, dataFilePath);

            await CreateFolderIfNotExistsAsync(projectFolderPath, DocumentsFolderName);
        }

        /// <summary>
        /// Creates a new ProjectLocal or if one exists, then update its primary data.
        /// </summary>
        /// <returns>
        /// If the returned ProjectLocal has a DateTime.MinValue, then the project did not already exist. 
        /// Otherwise the ProjectLocal with the original DateTime value is returned. 
        /// </returns>
        private async Task<ProjectLocal> CreateOrUpdateProjectLocalAsync(ProjectDto projectDto)
        {
            var existingProject = await GetProjectByIdAsync(projectDto.Id);
            if (existingProject == null)
            {
                // Create
                var projectLocal = new ProjectLocal();

                projectLocal.ProjectKeyInfoDto = projectDto;
                projectLocal.LastChangedTimestampForDocumentFolder = DateTime.MinValue;

                return projectLocal;
            }

            // Update - we don not update timestamp, since this must be done else where if documents are downloaded
            existingProject.ProjectKeyInfoDto = projectDto;
            return existingProject;
        }

        private Task DeleteProjectDocumentsFolderAsync(string projectId)
        {
            var documentsFolderPath = BuildDocumentsFolderPath(projectId);

            return FileSystem.DeleteFolderIfExistsAsync(documentsFolderPath);
        }

        /// <summary>
        /// Deletes all document folders except for the 5 newest.
        /// </summary>
        private async Task CleanProjectDocumentFolderAsync()
        {
            var projectLocalList = await GetProjectsAsync();

            if (projectLocalList.Count > MaxNumberOfDocumentFolders)
            {
                var orderedProjectLocalListDescending = projectLocalList
                    .OrderByDescending(x => x.LastChangedTimestampForDocumentFolder)
                    .ToList();

                var projectLocalListToDelete = orderedProjectLocalListDescending
                    .Skip(MaxNumberOfDocumentFolders);

                foreach (var projectLocal in projectLocalListToDelete)
                {
                    await DeleteProjectDocumentsFolderAsync(projectLocal.ProjectKeyInfoDto.Id);
                }
            }
        }

        private async Task CreateArchivedObservationsFolders(ArchivedSupervisionLocal archivedSupervisionLocal, string archivedSupervisionFolderPath)
        {
            foreach (var archivedObservation in archivedSupervisionLocal.ArchivedSupervisionDto.ArchivedObservations)
            {
                var archivedObservationFolderName = EnsureValidFolderName(ArchivedObservationBaseFolderName + archivedObservation.ObservationId.ToString());
                await CreateFolderIfNotExistsAsync(archivedSupervisionFolderPath, archivedObservationFolderName);
            }
        }

        private async Task CreateFolderStructureAsync()
        {
            var rootFolderPath = await FileSystem.GetRootFolderPathAsync();
            UserFolderPath = await CreateFolderIfNotExistsAsync(rootFolderPath, UserBaseFolderName + Session.User.Initials);
            await CreateUploadedSupervisionsFileIfNotExistsAsync();

            AdHocObservationsFolderPath = await CreateFolderIfNotExistsAsync(UserFolderPath, AdHocObservationFolderName);
        }

        private async Task CreateUploadedSupervisionsFileIfNotExistsAsync()
        {
            var dataFilePath = BuildDataFormatFileName(Path.Combine(UserFolderPath, LastUploadedFileName));

            if (false == await FileSystem.FileExistsAsync(dataFilePath))
            {
                await WriteObjectToFileAsync(new List<SupervisionLocal>(), dataFilePath);
            }
        }

        private string BuildAdHocObservationFolderName(Guid newAdHocObservationId)
        {
            return EnsureValidFolderName(AdHocObservationBaseFolderName + newAdHocObservationId.ToString());
        }

        private string BuildProjectFolderPath(string projectId)
        {
            return EnsureValidFolderPath(Path.Combine(UserFolderPath, BuildProjectFolderName(projectId)));
        }

        private string BuildProjectFolderName(string projectId)
        {
            return EnsureValidFolderName(ProjectsBaseFolderName + projectId);
        }

        private string BuildDocumentsFolderPath(string projectId)
        {
            string invalidPathToRemove = "/Documents/..";
            var projectFolderPath = BuildProjectFolderPath(projectId);

            if (projectFolderPath.Contains(invalidPathToRemove))
            {
                int index = projectFolderPath.IndexOf(invalidPathToRemove);
                string cleanPath = (index < 0)
                    ? projectFolderPath
                    : projectFolderPath.Remove(index, invalidPathToRemove.Length);

                projectFolderPath = cleanPath;
            }

            return Path.Combine(projectFolderPath, DocumentsFolderName);
        }

        /// <summary>
        /// Build an attachment with a .png extension.
        /// </summary>
        private async Task<string> BuildAttachmentPath(Guid supervisionId, Guid observationId, Guid attachmentId)
        {
            var supervision = await GetOpenSupervisionByIdAsync(supervisionId);

            var projectFolderPath = BuildProjectFolderPath(supervision.SupervisionDto.ProjectId);

            var supervisionFolderName = BuildSupervisionFolderName(supervision.SupervisionDto.IdGeneratedOnApp);
            var supervisionFolderPath = await CreateFolderIfNotExistsAsync(projectFolderPath, supervisionFolderName);
            var observationFolderPath = await CreateFolderIfNotExistsAsync(supervisionFolderPath, BuildObservationFolderName(observationId));
            var attachmentPath = Path.Combine(observationFolderPath, attachmentId.ToString());
            return attachmentPath;
        }

        /// <summary>
        /// Build an archived attachment path.
        /// </summary>
        private async Task<string> BuildArchivedAttachmentPath(Guid supervisionId, Guid observationId, Guid attachmentId)
        {
            string archivedSupervisionFolderPath = await BuildArchivedSupervisionFolderPath(supervisionId);
            var observationFolderPath = await CreateFolderIfNotExistsAsync(archivedSupervisionFolderPath, BuildArchivedObservationFolderName(observationId));
            var attachmentPath = Path.Combine(observationFolderPath, attachmentId.ToString());
            return attachmentPath;
        }

        private async Task<string> BuildArchivedSupervisionFolderPath(Guid supervisionId)
        {
            var archivedSupervisionFolderName = BuildArchivedSupervisionFolderName(supervisionId);
            var archivedSupervisionFolderPath = await CreateFolderIfNotExistsAsync(UserFolderPath, archivedSupervisionFolderName);
            return archivedSupervisionFolderPath;
        }

        private string BuildDocumentFileName(DocumentKeyInfoDto documentDto)
        {
            return EnsureValidFolderName(documentDto.Title);
        }

        private string BuildSupervisionFolderName(Guid supervisionId)
        {
            return EnsureValidFolderName(SupervisionBaseFolderName + supervisionId.ToString());
        }

        private string BuildArchivedSupervisionFolderName(Guid supervisionId)
        {
            return EnsureValidFolderName(ArchivedSupervisionBaseFolderName + supervisionId.ToString());
        }

        private string BuildArchivedObservationFolderName(Guid archivedObservationId)
        {
            return EnsureValidFolderName(ArchivedObservationBaseFolderName + archivedObservationId.ToString());
        }

        private string BuildObservationFolderName(Guid observationId)
        {
            return EnsureValidFolderName(ObservationBaseFolderName + observationId.ToString());
        }

        /// <summary>
        /// Returns object from "raw data file" if it exists; otherwise returns "default(TObject)".
        /// </summary>
        private async Task<TObject> GetObjectByIdFolderNameOrDefaultAsync<TObject>(string folderName)
            where TObject : class, new()
        {
            var folderPaths = await FileSystem.SearchFolderPathsInFolderAsync(UserFolderPath, folderName);
            if (folderPaths.Any())
            {
                return await ReadObjectFromFileOrDefaultAsync<TObject>(Path.Combine(folderPaths[0], BuildDataFormatFileName(RawDataFileName)));
            }

            return default(TObject);
        }

        private async Task WriteObjectByIdFolderNameAsync<TInput>(string folderName, TInput input)
        {
            var folderPaths = await FileSystem.SearchFolderPathsInFolderAsync(UserFolderPath, folderName);
            if (folderPaths.Any())
            {
                await WriteObjectToFileAsync(input, Path.Combine(folderPaths[0], BuildDataFormatFileName(RawDataFileName)));
            }
        }

        private async Task DeleteObjectByIdFolderNameIsExistsAsync(string folderName)
        {
            var folderPaths = await FileSystem.SearchFolderPathsInFolderAsync(UserFolderPath, folderName);
            if (folderPaths.Any())
            {
                await FileSystem.DeleteFolderIfExistsAsync(folderPaths[0]);
            }
        }
    }
}