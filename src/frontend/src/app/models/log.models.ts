export interface SimulatorStatus {
  currentSpeed: string;
}

export interface SpeedUpdateResponse {
  message: string;
  currentSpeed: string;
}

export interface Log {
  id: number;
  level: number;
  levelString?: string;
  message: string;
  exceptionDetails?: string;
  sourceService: string;
  timestamp: string;
}