import IServerDetailsResponse from './IServerDetailsResponse';

export default interface IServerListResponse {
    success: boolean;
    error: string | null;
    servers: IServerDetailsResponse[] | null;
}