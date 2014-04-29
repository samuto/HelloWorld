using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using SlimDX;
using WindowsFormsApplication7.Business.Profiling;
using WindowsFormsApplication7.Business.Geometry;
using WindowsFormsApplication7.CrossCutting;

namespace WindowsFormsApplication7.Business
{
    class ChunkColumn
    {
        public PositionChunk Position;
        public ColumnStageEnum Stage = ColumnStageEnum.NotGenerated;
        public bool Active;
        public enum ColumnStageEnum
        {
            NotGenerated,
            Generated,
            AllNeighborsGenerated,
            Decorated
        }

        public ChunkColumn(int x, int z)
        {
            Position = new PositionChunk(x, 0, z);
        }

        internal void InitializeStage()
        {
            ChunkCache cache = World.Instance.GetCachedChunks();

            // Check if we can progress to next stage
            List<Chunk.ChunkStageEnum> chunkStagesFound = new List<Chunk.ChunkStageEnum>();
            for (int y = 0; y < Chunk.MaxSizeY / 16f; y++)
            {
                Chunk chunk = cache.GetChunk(new PositionChunk(Position.X, y, Position.Z));
                if (!chunkStagesFound.Contains(chunk.Stage))
                {
                    chunkStagesFound.Add(chunk.Stage);
                }
            }
            if (chunkStagesFound.Contains(Chunk.ChunkStageEnum.NotGenerated))
            {
                Stage = ColumnStageEnum.NotGenerated;
            }
            else if (chunkStagesFound.Contains(Chunk.ChunkStageEnum.Generated))
            {
                Stage = ColumnStageEnum.Generated;
                NotifyNeighbors();
            }
            else
            {
                Stage = ColumnStageEnum.Decorated;
                NotifyNeighbors();
            }

        }

        internal void InvalidateMeAndNeighbors()
        {
            ChunkCache cache = World.Instance.GetCachedChunks();
            Invalidate();
            foreach (ChunkColumn column in cache.AllNeighborColumns(this))
            {
                column.Invalidate();
            }
        }

        private void Invalidate()
        {
            ChunkCache cache = World.Instance.GetCachedChunks();
            for (int y = 0; y < Chunk.MaxSizeY / 16f; y++)
            {
                Chunk chunk = cache.GetChunk(new PositionChunk(Position.X, y, Position.Z));
                chunk.Invalidate();
            }
        }

        internal void OnChunkGenerated()
        {
            ChunkCache cache = World.Instance.GetCachedChunks();
            if (Stage == ColumnStageEnum.NotGenerated)
            {
                // Check if we can progress to next stage
                for (int y = 0; y < Chunk.MaxSizeY / 16f; y++)
                {
                    Chunk chunk = cache.GetChunk(new PositionChunk(Position.X, y, Position.Z));
                    if (chunk.Stage == Chunk.ChunkStageEnum.NotGenerated)
                        return;
                }
                Stage = ColumnStageEnum.Generated;
                NotifyNeighbors();
            }
            else
            {
                throw new Exception("ERROR in chunkcolumn stage!!!");
            }

        }

        private void NotifyNeighbors()
        {
            ChunkCache cache = World.Instance.GetCachedChunks();
            OnNeighborGenerated();
            foreach (var neighborColumn in cache.AllNeighborColumns(this))
            {
                if (neighborColumn == null)
                    continue;
                neighborColumn.OnNeighborGenerated();
            }
        }



        private void OnNeighborGenerated()
        {
            if (Stage != ColumnStageEnum.Generated)
            {
                return;
            }
            ChunkCache cache = World.Instance.GetCachedChunks();
            var allNeighborColumns = cache.AllNeighborColumns(this);
            foreach (var column in allNeighborColumns)
            {
                if (column == null || column.Stage == ColumnStageEnum.NotGenerated)
                {
                    return;
                }
            }
            Stage = ColumnStageEnum.AllNeighborsGenerated;
        }

        internal void Decorate()
        {
            // Decorate here..
            ChunkCache cache = World.Instance.GetCachedChunks();
            Chunk chunk = cache.GetChunk(new PositionChunk(Position.X, 0, Position.Z));
            World.Instance.Generator(chunk).Decorate(chunk);
            InvalidateMeAndNeighbors();

            // update chunks and column stages
            SetChunkStage(Chunk.ChunkStageEnum.Update);
            Stage = ColumnStageEnum.Decorated;
            NotifyNeighbors();
        }

        private void SetChunkStage(Chunk.ChunkStageEnum newStage)
        {
            ChunkCache cache = World.Instance.GetCachedChunks();
            for (int y = 0; y < Chunk.MaxSizeY / 16f; y++)
            {
                Chunk chunk = cache.GetChunk(new PositionChunk(Position.X, y, Position.Z));
                chunk.Stage = newStage;
            }
        }
    }
}